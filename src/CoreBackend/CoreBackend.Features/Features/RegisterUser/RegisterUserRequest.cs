namespace CoreBackend.Features.RegisterUser;

public class RegisterUserRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
