namespace CoreBackend.Features.DeleteUser;

public class DeleteUserRequest
{
    public required string UserId { get; init; }
    public required string Password { get; init; }
}
