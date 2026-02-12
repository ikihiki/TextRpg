using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.GetUserProfile;

public class GetUserProfile : IUseCase<GetUserProfileRequest, GetUserProfileResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfile(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<GetUserProfileResponse> ExecuteAsync(GetUserProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.UserId, out var userGuid))
        {
            return new GetUserProfileResponse
            {
                Success = false,
                ErrorMessage = "Invalid user ID format"
            };
        }

        var user = await _userRepository.GetByIdAsync(UserId.From(userGuid), cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return new GetUserProfileResponse
            {
                Success = false,
                ErrorMessage = "User not found"
            };
        }

        return GetUserProfileResponse.FromUser(user);
    }
}
