using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.DeleteUser;

public class DeleteUser : IUseCase<DeleteUserRequest, DeleteUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public DeleteUser(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<DeleteUserResponse> ExecuteAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
        {
            return new DeleteUserResponse
            {
                Success = false,
                ErrorMessage = "Invalid user ID format"
            };
        }

        var user = await _userRepository.GetByIdAsync(UserId.From(userGuid), cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return new DeleteUserResponse
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }

        // Verify password for re-authentication
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return new DeleteUserResponse
            {
                Success = false,
                ErrorMessage = "Invalid password"
            };
        }

        // Mark user as deleted (soft delete)
        user.MarkAsDeleted();
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new DeleteUserResponse { Success = true };
    }
}
