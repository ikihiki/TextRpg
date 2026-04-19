using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace BffGateway.Service.Auth;

public static class BffAuthenticationExtensions
{
    public static IServiceCollection AddBffAuthentication(this IServiceCollection services, BffAuthOptions authOptions)
    {
        var authentication = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

        authentication.AddCookie(options =>
        {
            options.Cookie.Name = authOptions.SessionCookieName;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(12);
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                }
            };
        });

        if (authOptions.Google.IsConfigured)
        {
            authentication.AddGoogle(BffAuthOptions.GoogleProvider, options =>
            {
                options.ClientId = authOptions.Google.ClientId!;
                options.ClientSecret = authOptions.Google.ClientSecret!;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Events = BuildOAuthEvents(BffAuthOptions.GoogleProvider);
            });
        }

        if (authOptions.Microsoft.IsConfigured)
        {
            authentication.AddMicrosoftAccount(BffAuthOptions.MicrosoftProvider, options =>
            {
                options.ClientId = authOptions.Microsoft.ClientId!;
                options.ClientSecret = authOptions.Microsoft.ClientSecret!;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Events = BuildOAuthEvents(BffAuthOptions.MicrosoftProvider);
            });
        }

        services.AddAuthorization();
        return services;
    }

    private static Func<RemoteFailureContext, Task> BuildRemoteFailureHandler(string provider)
    {
        return context =>
        {
            var returnPath = context.Properties?.Items.TryGetValue("returnUrl", out var configuredReturnUrl) == true
                ? configuredReturnUrl
                : "/";
            var message = context.Failure?.Message ?? "OAuth authentication failed.";
            var target = $"/api/auth/oauth/error?provider={Uri.EscapeDataString(provider)}&returnUrl={Uri.EscapeDataString(returnPath ?? "/")}&message={Uri.EscapeDataString(message)}";

            context.Response.Redirect(target);
            context.HandleResponse();
            return Task.CompletedTask;
        };
    }

    private static OAuthEvents BuildOAuthEvents(string provider)
    {
        return new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                context.Identity?.AddClaim(new Claim(AuthPrincipalFactory.ProviderClaimType, provider));
                return Task.CompletedTask;
            },
            OnTicketReceived = context =>
            {
                context.Principal = AuthPrincipalFactory.CreateExternalPrincipal(context.Principal!, provider);
                return Task.CompletedTask;
            },
            OnRemoteFailure = BuildRemoteFailureHandler(provider)
        };
    }
}
