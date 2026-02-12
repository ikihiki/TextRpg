using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextRpg.Core;

// Feature aliases
using FeatureRegisterUser = CoreBackend.Features.RegisterUser;
using FeatureLoginUser = CoreBackend.Features.LoginUser;
using FeatureLogoutUser = CoreBackend.Features.LogoutUser;
using FeatureGetUserProfile = CoreBackend.Features.GetUserProfile;
using FeatureUpdateUserProfile = CoreBackend.Features.UpdateUserProfile;
using FeatureDeleteUser = CoreBackend.Features.DeleteUser;
using FeatureValidateSession = CoreBackend.Features.ValidateSession;

namespace CoreBackend.Service.Services;

public class UserServiceImpl : UserService.UserServiceBase
{
    private readonly FeatureRegisterUser.RegisterUser _registerUser;
    private readonly FeatureLoginUser.LoginUser _loginUser;
    private readonly FeatureLogoutUser.LogoutUser _logoutUser;
    private readonly FeatureGetUserProfile.GetUserProfile _getUserProfile;
    private readonly FeatureUpdateUserProfile.UpdateUserProfile _updateUserProfile;
    private readonly FeatureDeleteUser.DeleteUser _deleteUser;
    private readonly FeatureValidateSession.ValidateSession _validateSession;

    public UserServiceImpl(
        FeatureRegisterUser.RegisterUser registerUser,
        FeatureLoginUser.LoginUser loginUser,
        FeatureLogoutUser.LogoutUser logoutUser,
        FeatureGetUserProfile.GetUserProfile getUserProfile,
        FeatureUpdateUserProfile.UpdateUserProfile updateUserProfile,
        FeatureDeleteUser.DeleteUser deleteUser,
        FeatureValidateSession.ValidateSession validateSession)
    {
        _registerUser = registerUser;
        _loginUser = loginUser;
        _logoutUser = logoutUser;
        _getUserProfile = getUserProfile;
        _updateUserProfile = updateUserProfile;
        _deleteUser = deleteUser;
        _validateSession = validateSession;
    }

    public override async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request, ServerCallContext context)
    {
        var result = await _registerUser.ExecuteAsync(new FeatureRegisterUser.RegisterUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, context.CancellationToken);

        return new RegisterUserResponse
        {
            Success = result.Success,
            UserId = result.UserId ?? string.Empty,
            ErrorMessage = result.ErrorMessage ?? string.Empty
        };
    }

    public override async Task<LoginUserResponse> LoginUser(LoginUserRequest request, ServerCallContext context)
    {
        var result = await _loginUser.ExecuteAsync(new FeatureLoginUser.LoginUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, context.CancellationToken);

        var response = new LoginUserResponse
        {
            Success = result.Success,
            UserId = result.UserId ?? string.Empty,
            SessionToken = result.SessionToken ?? string.Empty,
            ErrorMessage = result.ErrorMessage ?? string.Empty
        };

        if (result.ExpiresAt.HasValue)
        {
            response.ExpiresAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.ExpiresAt.Value, DateTimeKind.Utc));
        }

        return response;
    }

    public override async Task<LogoutUserResponse> LogoutUser(LogoutUserRequest request, ServerCallContext context)
    {
        var result = await _logoutUser.ExecuteAsync(new FeatureLogoutUser.LogoutUserRequest
        {
            SessionToken = request.SessionToken
        }, context.CancellationToken);

        return new LogoutUserResponse
        {
            Success = result.Success
        };
    }

    public override async Task<GetUserProfileResponse> GetUserProfile(GetUserProfileRequest request, ServerCallContext context)
    {
        var result = await _getUserProfile.ExecuteAsync(new FeatureGetUserProfile.GetUserProfileRequest
        {
            UserId = request.UserId
        }, context.CancellationToken);

        var response = new GetUserProfileResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage ?? string.Empty
        };

        if (result.Success && result.UserId != null)
        {
            response.Profile = new UserProfile
            {
                UserId = result.UserId,
                Email = result.Email ?? string.Empty,
                DisplayName = result.DisplayName ?? string.Empty,
                IconUrl = result.IconUrl ?? string.Empty,
                Bio = result.Bio ?? string.Empty,
                Language = result.Language ?? string.Empty,
                NotificationSettings = new NotificationSettings
                {
                    NoteUpdates = result.NoteUpdates,
                    SessionReminders = result.SessionReminders,
                    Marketing = result.Marketing
                },
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.CreatedAt ?? DateTime.UtcNow, DateTimeKind.Utc)),
                UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.UpdatedAt ?? DateTime.UtcNow, DateTimeKind.Utc))
            };
        }

        return response;
    }

    public override async Task<UpdateUserProfileResponse> UpdateUserProfile(UpdateUserProfileRequest request, ServerCallContext context)
    {
        var result = await _updateUserProfile.ExecuteAsync(new FeatureUpdateUserProfile.UpdateUserProfileRequest
        {
            UserId = request.UserId,
            DisplayName = request.DisplayName,
            IconUrl = string.IsNullOrEmpty(request.IconUrl) ? null : request.IconUrl,
            Bio = string.IsNullOrEmpty(request.Bio) ? null : request.Bio,
            Language = request.Language,
            NoteUpdates = request.NotificationSettings?.NoteUpdates ?? true,
            SessionReminders = request.NotificationSettings?.SessionReminders ?? true,
            Marketing = request.NotificationSettings?.Marketing ?? false
        }, context.CancellationToken);

        var response = new UpdateUserProfileResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage ?? string.Empty
        };

        if (result.Success && result.UserId != null)
        {
            response.Profile = new UserProfile
            {
                UserId = result.UserId,
                Email = result.Email ?? string.Empty,
                DisplayName = result.DisplayName ?? string.Empty,
                IconUrl = result.IconUrl ?? string.Empty,
                Bio = result.Bio ?? string.Empty,
                Language = result.Language ?? string.Empty,
                NotificationSettings = new NotificationSettings
                {
                    NoteUpdates = result.NoteUpdates,
                    SessionReminders = result.SessionReminders,
                    Marketing = result.Marketing
                },
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.CreatedAt ?? DateTime.UtcNow, DateTimeKind.Utc)),
                UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.UpdatedAt ?? DateTime.UtcNow, DateTimeKind.Utc))
            };
        }

        return response;
    }

    public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
    {
        var result = await _deleteUser.ExecuteAsync(new FeatureDeleteUser.DeleteUserRequest
        {
            UserId = request.UserId,
            Password = request.Password
        }, context.CancellationToken);

        return new DeleteUserResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage ?? string.Empty
        };
    }

    public override async Task<ValidateSessionResponse> ValidateSession(ValidateSessionRequest request, ServerCallContext context)
    {
        var result = await _validateSession.ExecuteAsync(new FeatureValidateSession.ValidateSessionRequest
        {
            SessionToken = request.SessionToken
        }, context.CancellationToken);

        var response = new ValidateSessionResponse
        {
            Valid = result.Valid,
            UserId = result.UserId ?? string.Empty
        };

        if (result.ExpiresAt.HasValue)
        {
            response.ExpiresAt = Timestamp.FromDateTime(DateTime.SpecifyKind(result.ExpiresAt.Value, DateTimeKind.Utc));
        }

        return response;
    }
}
