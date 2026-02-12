using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.LogoutUser;

public class LogoutUser : IUseCase<LogoutUserRequest, LogoutUserResponse>
{
    private readonly IUserSessionRepository _sessionRepository;

    public LogoutUser(IUserSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async ValueTask<LogoutUserResponse> ExecuteAsync(LogoutUserRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByTokenAsync(request.SessionToken, cancellationToken);

        if (session == null)
        {
            return new LogoutUserResponse { Success = true };
        }

        session.Revoke();
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return new LogoutUserResponse { Success = true };
    }
}
