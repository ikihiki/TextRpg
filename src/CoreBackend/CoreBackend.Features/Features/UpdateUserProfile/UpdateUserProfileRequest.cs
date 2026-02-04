namespace CoreBackend.Features.UpdateUserProfile;

public class UpdateUserProfileRequest
{
    public required string UserId { get; init; }
    public required string DisplayName { get; init; }
    public string? IconUrl { get; init; }
    public string? Bio { get; init; }
    public required string Language { get; init; }
    public required bool NoteUpdates { get; init; }
    public required bool SessionReminders { get; init; }
    public required bool Marketing { get; init; }
}
