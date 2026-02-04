using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.UpdateUserProfile;

public class UpdateUserProfile : IUseCase<UpdateUserProfileRequest, UpdateUserProfileResponse>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfile(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<UpdateUserProfileResponse> ExecuteAsync(UpdateUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
        {
            return new UpdateUserProfileResponse
            {
                Success = false,
                ErrorMessage = "Invalid user ID format"
            };
        }

        var user = await _userRepository.GetByIdAsync(UserId.From(userGuid), cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return new UpdateUserProfileResponse
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }

        // Validate display name
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return new UpdateUserProfileResponse
            {
                Success = false,
                ErrorMessage = "Display name is required"
            };
        }

        // Update profile
        user.UpdateProfile(
            request.DisplayName,
            request.IconUrl,
            request.Bio,
            request.Language,
            new NotificationSettings
            {
                NoteUpdates = request.NoteUpdates,
                SessionReminders = request.SessionReminders,
                Marketing = request.Marketing
            });

        await _userRepository.UpdateAsync(user, cancellationToken);

        return UpdateUserProfileResponse.FromUser(user);
    }
}
