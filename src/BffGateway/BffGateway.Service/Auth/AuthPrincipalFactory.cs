using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BffGateway.Service.Auth;

public static class AuthPrincipalFactory
{
    public const string ProviderClaimType = "textrpg:auth_provider";
    public const string ProviderSubjectClaimType = "textrpg:provider_subject";

    public static ClaimsPrincipal CreateBootstrapPrincipal(BffAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Bootstrap.Email))
        {
            throw new InvalidOperationException("Bootstrap email is not configured.");
        }

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"bootstrap:{options.Bootstrap.Email.ToLowerInvariant()}"));
        identity.AddClaim(new Claim(ClaimTypes.Name, options.Bootstrap.DisplayName));
        identity.AddClaim(new Claim(ClaimTypes.Email, options.Bootstrap.Email));
        identity.AddClaim(new Claim(ProviderClaimType, BffAuthOptions.PasswordProvider));

        return new ClaimsPrincipal(identity);
    }

    public static ClaimsPrincipal CreateExternalPrincipal(ClaimsPrincipal externalPrincipal, string provider)
    {
        var subject = externalPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? externalPrincipal.FindFirstValue("sub")
            ?? throw new InvalidOperationException($"OAuth provider '{provider}' did not return a subject identifier.");

        var email = externalPrincipal.FindFirstValue(ClaimTypes.Email)
            ?? externalPrincipal.FindFirstValue("email");
        var displayName = externalPrincipal.FindFirstValue(ClaimTypes.Name)
            ?? externalPrincipal.FindFirstValue("name")
            ?? email
            ?? $"{provider} user";

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"{provider}:{subject}"));
        identity.AddClaim(new Claim(ClaimTypes.Name, displayName));
        identity.AddClaim(new Claim(ProviderClaimType, provider));
        identity.AddClaim(new Claim(ProviderSubjectClaimType, subject));

        if (!string.IsNullOrWhiteSpace(email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
        }

        return new ClaimsPrincipal(identity);
    }
}
