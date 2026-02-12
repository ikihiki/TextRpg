namespace CoreBackend.Features.LoginUser;

public class LoginUserResponse
{
    public required bool Success { get; init; }
    public string? UserId { get; init; }
    public string? SessionToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? ErrorMessage { get; init; }
}
