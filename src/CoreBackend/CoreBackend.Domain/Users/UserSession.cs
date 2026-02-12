namespace CoreBackend.Domain.Users;

/// <summary>
/// ユーザーセッション（認証セッション）のエンティティ
/// </summary>
public class UserSession
{
    public Guid Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private UserSession() { }

    public static UserSession Create(UserId userId, TimeSpan expiration)
    {
        var now = DateTime.UtcNow;
        return new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = GenerateToken(),
            CreatedAt = now,
            ExpiresAt = now.Add(expiration),
            IsRevoked = false
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsValid() => !IsRevoked && DateTime.UtcNow < ExpiresAt;

    private static string GenerateToken()
    {
        var bytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
