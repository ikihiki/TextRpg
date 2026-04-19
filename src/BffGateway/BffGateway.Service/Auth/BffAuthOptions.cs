namespace BffGateway.Service.Auth;

public sealed class BffAuthOptions
{
    public const string PasswordProvider = "password";
    public const string GoogleProvider = "google";
    public const string MicrosoftProvider = "microsoft";

    public string FrontendUrl { get; init; } = "http://localhost:5173";
    public string[] AllowedOrigins { get; init; } = ["http://localhost:5173"];
    public string SessionCookieName { get; init; } = "TextRpg.Auth";
    public BootstrapCredentialOptions Bootstrap { get; init; } = new();
    public OAuthProviderOptions Google { get; init; } = new();
    public OAuthProviderOptions Microsoft { get; init; } = new();

    public bool IsPasswordLoginEnabled =>
        !string.IsNullOrWhiteSpace(Bootstrap.Email) &&
        !string.IsNullOrWhiteSpace(Bootstrap.Password);

    public IReadOnlyList<OAuthProviderDescriptor> EnabledOAuthProviders
    {
        get
        {
            var providers = new List<OAuthProviderDescriptor>();

            if (Google.IsConfigured)
            {
                providers.Add(new OAuthProviderDescriptor(GoogleProvider, Google.DisplayName));
            }

            if (Microsoft.IsConfigured)
            {
                providers.Add(new OAuthProviderDescriptor(MicrosoftProvider, Microsoft.DisplayName));
            }

            return providers;
        }
    }

    public static BffAuthOptions FromConfiguration(IConfiguration configuration)
    {
        var frontendUrl = configuration["AUTH_FRONTEND_URL"];
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            frontendUrl = "http://localhost:5173";
        }

        if (!Uri.TryCreate(frontendUrl, UriKind.Absolute, out var frontendUri))
        {
            throw new InvalidOperationException("AUTH_FRONTEND_URL must be an absolute URL.");
        }

        var allowedOrigins = configuration["AUTH_ALLOWED_ORIGINS"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        allowedOrigins ??= [frontendUri.GetLeftPart(UriPartial.Authority)];

        foreach (var origin in allowedOrigins)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException($"AUTH_ALLOWED_ORIGINS contains an invalid absolute URL: {origin}");
            }
        }

        return new BffAuthOptions
        {
            FrontendUrl = frontendUri.ToString().TrimEnd('/'),
            AllowedOrigins = allowedOrigins,
            SessionCookieName = configuration["AUTH_SESSION_COOKIE_NAME"] ?? "TextRpg.Auth",
            Bootstrap = new BootstrapCredentialOptions
            {
                Email = configuration["AUTH_BOOTSTRAP_EMAIL"],
                Password = configuration["AUTH_BOOTSTRAP_PASSWORD"],
                DisplayName = configuration["AUTH_BOOTSTRAP_DISPLAY_NAME"] ?? "Bootstrap User"
            },
            Google = new OAuthProviderOptions
            {
                ClientId = configuration["AUTH_GOOGLE_CLIENT_ID"],
                ClientSecret = configuration["AUTH_GOOGLE_CLIENT_SECRET"],
                DisplayName = configuration["AUTH_GOOGLE_DISPLAY_NAME"] ?? "Google"
            },
            Microsoft = new OAuthProviderOptions
            {
                ClientId = configuration["AUTH_MICROSOFT_CLIENT_ID"],
                ClientSecret = configuration["AUTH_MICROSOFT_CLIENT_SECRET"],
                DisplayName = configuration["AUTH_MICROSOFT_DISPLAY_NAME"] ?? "Microsoft"
            }
        };
    }

    public bool TryGetOAuthProvider(string provider, out OAuthProviderDescriptor descriptor)
    {
        descriptor = EnabledOAuthProviders.FirstOrDefault(candidate =>
            string.Equals(candidate.Id, provider, StringComparison.OrdinalIgnoreCase))
            ?? new OAuthProviderDescriptor(string.Empty, string.Empty);

        return !string.IsNullOrWhiteSpace(descriptor.Id);
    }

    public string BuildFrontendRedirect(string returnPath, IReadOnlyDictionary<string, string?> query)
    {
        var sanitizedPath = SanitizeReturnPath(returnPath);
        var builder = new UriBuilder($"{FrontendUrl}{sanitizedPath}");

        var queryPairs = query
            .Where(entry => !string.IsNullOrWhiteSpace(entry.Value))
            .Select(entry => $"{Uri.EscapeDataString(entry.Key)}={Uri.EscapeDataString(entry.Value!)}");

        builder.Query = string.Join("&", queryPairs);
        return builder.Uri.ToString();
    }

    public static string SanitizeReturnPath(string? returnPath)
    {
        if (string.IsNullOrWhiteSpace(returnPath))
        {
            return "/";
        }

        if (!returnPath.StartsWith("/", StringComparison.Ordinal))
        {
            return "/";
        }

        return returnPath;
    }
}

public sealed class BootstrapCredentialOptions
{
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string DisplayName { get; init; } = "Bootstrap User";
}

public sealed class OAuthProviderOptions
{
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public string DisplayName { get; init; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(ClientSecret);
}

public sealed record OAuthProviderDescriptor(string Id, string DisplayName);
