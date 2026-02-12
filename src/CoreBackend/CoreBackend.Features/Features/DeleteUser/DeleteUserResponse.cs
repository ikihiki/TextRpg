namespace CoreBackend.Features.DeleteUser;

public class DeleteUserResponse
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
