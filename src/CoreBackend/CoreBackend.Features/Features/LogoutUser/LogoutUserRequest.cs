namespace CoreBackend.Features.LogoutUser;

public class LogoutUserRequest
{
    public required string SessionToken { get; init; }
}
