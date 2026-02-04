namespace CoreBackend.Domain.Users;

/// <summary>
/// ユーザーの集約ルートエンティティ
/// </summary>
public class User
{
    public UserId Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? IconUrl { get; private set; }
    public string? Bio { get; private set; }
    public string Language { get; private set; } = "ja";
    public NotificationSettings NotificationSettings { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private User() { }

    public static User Create(string email, string passwordHash)
    {
        var now = DateTime.UtcNow;
        return new User
        {
            Id = UserId.New(),
            Email = email,
            PasswordHash = passwordHash,
            DisplayName = email.Split('@')[0],
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };
    }

    public void UpdateProfile(string displayName, string? iconUrl, string? bio, string language, NotificationSettings notificationSettings)
    {
        DisplayName = displayName;
        IconUrl = iconUrl;
        Bio = bio;
        Language = language;
        NotificationSettings = notificationSettings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// 通知設定の値オブジェクト
/// </summary>
public record NotificationSettings
{
    public bool NoteUpdates { get; init; } = true;
    public bool SessionReminders { get; init; } = true;
    public bool Marketing { get; init; } = false;
}
