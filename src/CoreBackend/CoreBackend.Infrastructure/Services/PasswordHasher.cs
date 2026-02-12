using CoreBackend.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace CoreBackend.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();
    private static readonly object _dummy = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(_dummy, password);
    }

    public bool Verify(string password, string hash)
    {
        var result = _hasher.VerifyHashedPassword(_dummy, hash, password);
        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
