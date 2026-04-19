using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BffGateway.Service.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapGet("/providers", (BffAuthOptions authOptions) =>
        {
            var response = new AuthProvidersResponse(
                authOptions.IsPasswordLoginEnabled,
                authOptions.EnabledOAuthProviders
                    .Select(provider => new OAuthProviderResponse(provider.Id, provider.DisplayName))
                    .ToArray());

            return Results.Ok(response);
        });

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var response = user.Identity?.IsAuthenticated == true
                ? new AuthStateResponse(
                    true,
                    new AuthUserResponse(
                        user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                        user.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                        user.FindFirstValue(ClaimTypes.Email),
                        user.FindFirstValue(AuthPrincipalFactory.ProviderClaimType) ?? BffAuthOptions.PasswordProvider))
                : new AuthStateResponse(false, null);

            return Results.Ok(response);
        });

        group.MapPost("/login", async (PasswordLoginRequest? request, HttpContext context, BootstrapPasswordLoginService loginService, BffAuthOptions authOptions) =>
        {
            if (!loginService.IsEnabled)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "Password login is not configured.",
                    detail: "Set AUTH_BOOTSTRAP_EMAIL and AUTH_BOOTSTRAP_PASSWORD to enable password login.");
            }

            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["email"] = ["Email is required."],
                    ["password"] = ["Password is required."]
                });
            }

            if (!loginService.Validate(request.Email, request.Password))
            {
                return Results.Json(new ErrorResponse("メールアドレスまたはパスワードが正しくありません。"), statusCode: StatusCodes.Status401Unauthorized);
            }

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                AuthPrincipalFactory.CreateBootstrapPrincipal(authOptions));

            return Results.Ok(new AuthStateResponse(
                true,
                new AuthUserResponse(
                    $"bootstrap:{authOptions.Bootstrap.Email!.ToLowerInvariant()}",
                    authOptions.Bootstrap.DisplayName,
                    authOptions.Bootstrap.Email,
                    BffAuthOptions.PasswordProvider)));
        });

        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.NoContent();
        });

        group.MapGet("/oauth/{provider}", (string provider, string? returnUrl, BffAuthOptions authOptions) =>
        {
            if (!authOptions.TryGetOAuthProvider(provider, out _))
            {
                return Results.NotFound(new ErrorResponse($"OAuth provider '{provider}' is not configured."));
            }

            var sanitizedReturnUrl = BffAuthOptions.SanitizeReturnPath(returnUrl);
            var properties = new AuthenticationProperties
            {
                RedirectUri = $"/api/auth/oauth/post-login?provider={Uri.EscapeDataString(provider)}&returnUrl={Uri.EscapeDataString(sanitizedReturnUrl)}"
            };
            properties.Items["returnUrl"] = sanitizedReturnUrl;

            return Results.Challenge(properties, [provider]);
        });

        group.MapGet("/oauth/post-login", (string provider, string? returnUrl, ClaimsPrincipal user, BffAuthOptions authOptions) =>
        {
            var sanitizedReturnUrl = BffAuthOptions.SanitizeReturnPath(returnUrl);

            if (user.Identity?.IsAuthenticated != true)
            {
                return Results.Redirect(authOptions.BuildFrontendRedirect(sanitizedReturnUrl, new Dictionary<string, string?>
                {
                    ["auth"] = "error",
                    ["provider"] = provider,
                    ["message"] = "OAuth login did not complete."
                }));
            }

            return Results.Redirect(authOptions.BuildFrontendRedirect(sanitizedReturnUrl, new Dictionary<string, string?>
            {
                ["auth"] = "success",
                ["provider"] = provider
            }));
        });

        group.MapGet("/oauth/error", (string provider, string? returnUrl, string? message, BffAuthOptions authOptions) =>
        {
            var sanitizedReturnUrl = BffAuthOptions.SanitizeReturnPath(returnUrl);

            return Results.Redirect(authOptions.BuildFrontendRedirect(sanitizedReturnUrl, new Dictionary<string, string?>
            {
                ["auth"] = "error",
                ["provider"] = provider,
                ["message"] = message ?? "OAuth login failed."
            }));
        });

        return endpoints;
    }

    private sealed record AuthProvidersResponse(
        bool PasswordLoginEnabled,
        [property: JsonPropertyName("oauthProviders")]
        IReadOnlyList<OAuthProviderResponse> OAuthProviders);

    private sealed record OAuthProviderResponse(string Id, string DisplayName);

    private sealed record AuthStateResponse(bool Authenticated, AuthUserResponse? User);

    private sealed record AuthUserResponse(string UserId, string DisplayName, string? Email, string Provider);

    private sealed record ErrorResponse(string Message);

    public sealed record PasswordLoginRequest(string Email, string Password);
}
