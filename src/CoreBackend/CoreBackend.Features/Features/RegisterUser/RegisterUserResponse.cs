namespace CoreBackend.Features.RegisterUser;

public class RegisterUserResponse
{
    public required bool Success { get; init; }
    public string? UserId { get; init; }
    public string? ErrorMessage { get; init; }
}
