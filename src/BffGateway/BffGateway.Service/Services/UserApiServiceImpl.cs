using Grpc.Core;
using TextRpg.Bff;
using TextRpg.Core;
using CoreUserService = TextRpg.Core.UserService;

namespace BffGateway.Service.Services;

public class UserApiServiceImpl : UserApi.UserApiBase
{
    private readonly CoreUserService.UserServiceClient _coreUserService;

    public UserApiServiceImpl(CoreUserService.UserServiceClient coreUserService)
    {
        _coreUserService = coreUserService;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        // First register the user
        var registerResult = await _coreUserService.RegisterUserAsync(new RegisterUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken: context.CancellationToken);

        if (!registerResult.Success)
        {
            return new RegisterResponse
            {
                Success = false,
                ErrorMessage = registerResult.ErrorMessage
            };
        }

        // Then login to get session token
        var loginResult = await _coreUserService.LoginUserAsync(new LoginUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken: context.CancellationToken);

        return new RegisterResponse
        {
            Success = loginResult.Success,
            UserId = loginResult.UserId,
            SessionToken = loginResult.SessionToken,
            ExpiresAt = loginResult.ExpiresAt,
            ErrorMessage = loginResult.ErrorMessage
        };
    }

    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var result = await _coreUserService.LoginUserAsync(new LoginUserRequest
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken: context.CancellationToken);

        return new LoginResponse
        {
            Success = result.Success,
            UserId = result.UserId,
            SessionToken = result.SessionToken,
            ExpiresAt = result.ExpiresAt,
            ErrorMessage = result.ErrorMessage
        };
    }

    public override async Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
    {
        // Get session token from metadata
        var sessionToken = GetSessionToken(context);
        if (string.IsNullOrEmpty(sessionToken))
        {
            return new LogoutResponse { Success = false };
        }

        var result = await _coreUserService.LogoutUserAsync(new LogoutUserRequest
        {
            SessionToken = sessionToken
        }, cancellationToken: context.CancellationToken);

        return new LogoutResponse
        {
            Success = result.Success
        };
    }

    public override async Task<GetProfileResponse> GetProfile(GetProfileRequest request, ServerCallContext context)
    {
        // Get session token from metadata and validate
        var sessionToken = GetSessionToken(context);
        if (string.IsNullOrEmpty(sessionToken))
        {
            return new GetProfileResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var sessionValidation = await _coreUserService.ValidateSessionAsync(new ValidateSessionRequest
        {
            SessionToken = sessionToken
        }, cancellationToken: context.CancellationToken);

        if (!sessionValidation.Valid)
        {
            return new GetProfileResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var result = await _coreUserService.GetUserProfileAsync(new GetUserProfileRequest
        {
            UserId = sessionValidation.UserId
        }, cancellationToken: context.CancellationToken);

        if (!result.Success)
        {
            return new GetProfileResponse
            {
                Success = false,
                ErrorMessage = result.ErrorMessage
            };
        }

        return new GetProfileResponse
        {
            Success = true,
            Profile = new Profile
            {
                UserId = result.Profile.UserId,
                Email = result.Profile.Email,
                DisplayName = result.Profile.DisplayName,
                IconUrl = result.Profile.IconUrl,
                Bio = result.Profile.Bio,
                Language = result.Profile.Language,
                NotificationSettings = new ProfileNotificationSettings
                {
                    NoteUpdates = result.Profile.NotificationSettings.NoteUpdates,
                    SessionReminders = result.Profile.NotificationSettings.SessionReminders,
                    Marketing = result.Profile.NotificationSettings.Marketing
                },
                CreatedAt = result.Profile.CreatedAt,
                UpdatedAt = result.Profile.UpdatedAt
            }
        };
    }

    public override async Task<UpdateProfileResponse> UpdateProfile(UpdateProfileRequest request, ServerCallContext context)
    {
        // Get session token from metadata and validate
        var sessionToken = GetSessionToken(context);
        if (string.IsNullOrEmpty(sessionToken))
        {
            return new UpdateProfileResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var sessionValidation = await _coreUserService.ValidateSessionAsync(new ValidateSessionRequest
        {
            SessionToken = sessionToken
        }, cancellationToken: context.CancellationToken);

        if (!sessionValidation.Valid)
        {
            return new UpdateProfileResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var result = await _coreUserService.UpdateUserProfileAsync(new TextRpg.Core.UpdateUserProfileRequest
        {
            UserId = sessionValidation.UserId,
            DisplayName = request.DisplayName,
            IconUrl = request.IconUrl,
            Bio = request.Bio,
            Language = request.Language,
            NotificationSettings = new TextRpg.Core.NotificationSettings
            {
                NoteUpdates = request.NotificationSettings?.NoteUpdates ?? true,
                SessionReminders = request.NotificationSettings?.SessionReminders ?? true,
                Marketing = request.NotificationSettings?.Marketing ?? false
            }
        }, cancellationToken: context.CancellationToken);

        if (!result.Success)
        {
            return new UpdateProfileResponse
            {
                Success = false,
                ErrorMessage = result.ErrorMessage
            };
        }

        return new UpdateProfileResponse
        {
            Success = true,
            Profile = new Profile
            {
                UserId = result.Profile.UserId,
                Email = result.Profile.Email,
                DisplayName = result.Profile.DisplayName,
                IconUrl = result.Profile.IconUrl,
                Bio = result.Profile.Bio,
                Language = result.Profile.Language,
                NotificationSettings = new ProfileNotificationSettings
                {
                    NoteUpdates = result.Profile.NotificationSettings.NoteUpdates,
                    SessionReminders = result.Profile.NotificationSettings.SessionReminders,
                    Marketing = result.Profile.NotificationSettings.Marketing
                },
                CreatedAt = result.Profile.CreatedAt,
                UpdatedAt = result.Profile.UpdatedAt
            }
        };
    }

    public override async Task<DeleteAccountResponse> DeleteAccount(DeleteAccountRequest request, ServerCallContext context)
    {
        // Get session token from metadata and validate
        var sessionToken = GetSessionToken(context);
        if (string.IsNullOrEmpty(sessionToken))
        {
            return new DeleteAccountResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var sessionValidation = await _coreUserService.ValidateSessionAsync(new ValidateSessionRequest
        {
            SessionToken = sessionToken
        }, cancellationToken: context.CancellationToken);

        if (!sessionValidation.Valid)
        {
            return new DeleteAccountResponse
            {
                Success = false,
                ErrorMessage = "Unauthorized"
            };
        }

        var result = await _coreUserService.DeleteUserAsync(new TextRpg.Core.DeleteUserRequest
        {
            UserId = sessionValidation.UserId,
            Password = request.Password
        }, cancellationToken: context.CancellationToken);

        return new DeleteAccountResponse
        {
            Success = result.Success,
            ErrorMessage = result.ErrorMessage
        };
    }

    private static string? GetSessionToken(ServerCallContext context)
    {
        var authHeader = context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization");
        if (authHeader == null)
            return null;

        var value = authHeader.Value;
        if (value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return value.Substring(7);
        }

        return value;
    }
}
