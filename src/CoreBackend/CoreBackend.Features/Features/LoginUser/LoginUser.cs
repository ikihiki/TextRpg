using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.LoginUser;

public class LoginUser : IUseCase<LoginUserRequest, LoginUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private static readonly TimeSpan SessionExpiration = TimeSpan.FromDays(7);

    public LoginUser(
        IUserRepository userRepository,
        IUserSessionRepository sessionRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<LoginUserResponse> ExecuteAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || user.IsDeleted)
        {
            return new LoginUserResponse
            {
                Success = false,
                ErrorMessage = "Invalid email or password"
            };
        }

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return new LoginUserResponse
            {
                Success = false,
                ErrorMessage = "Invalid email or password"
            };
        }

        // Create session
        var session = UserSession.Create(user.Id, SessionExpiration);
        await _sessionRepository.AddAsync(session, cancellationToken);

        return new LoginUserResponse
        {
            Success = true,
            UserId = user.Id.ToString(),
            SessionToken = session.Token,
            ExpiresAt = session.ExpiresAt
        };
    }
}
