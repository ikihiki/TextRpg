using CoreBackend.Domain.Users;

namespace CoreBackend.Features.GetUserProfile;

public class GetUserProfileResponse
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    
    // Profile data (null when Success is false)
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public string? DisplayName { get; init; }
    public string? IconUrl { get; init; }
    public string? Bio { get; init; }
    public string? Language { get; init; }
    public bool NoteUpdates { get; init; }
    public bool SessionReminders { get; init; }
    public bool Marketing { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public static GetUserProfileResponse FromUser(User user)
    {
        return new GetUserProfileResponse
        {
            Success = true,
            UserId = user.Id.ToString(),
            Email = user.Email,
            DisplayName = user.DisplayName,
            IconUrl = user.IconUrl,
            Bio = user.Bio,
            Language = user.Language,
            NoteUpdates = user.NotificationSettings.NoteUpdates,
            SessionReminders = user.NotificationSettings.SessionReminders,
            Marketing = user.NotificationSettings.Marketing,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
