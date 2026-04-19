using System.Security.Cryptography;
using System.Text;

namespace BffGateway.Service.Auth;

public sealed class BootstrapPasswordLoginService(BffAuthOptions options)
{
    public bool IsEnabled => options.IsPasswordLoginEnabled;

    public bool Validate(string email, string password)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(options.Bootstrap.Email) || string.IsNullOrWhiteSpace(options.Bootstrap.Password))
        {
            return false;
        }

        if (!string.Equals(email.Trim(), options.Bootstrap.Email, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var configuredHash = SHA256.HashData(Encoding.UTF8.GetBytes(options.Bootstrap.Password));

        return CryptographicOperations.FixedTimeEquals(providedHash, configuredHash);
    }
}
