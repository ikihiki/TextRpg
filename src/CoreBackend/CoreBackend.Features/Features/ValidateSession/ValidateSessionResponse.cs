namespace CoreBackend.Features.ValidateSession;

public class ValidateSessionResponse
{
    public required bool Valid { get; init; }
    public string? UserId { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
