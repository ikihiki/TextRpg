using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.ValidateSession;

public class ValidateSession : IUseCase<ValidateSessionRequest, ValidateSessionResponse>
{
    private readonly IUserSessionRepository _sessionRepository;

    public ValidateSession(IUserSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async ValueTask<ValidateSessionResponse> ExecuteAsync(ValidateSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByTokenAsync(request.SessionToken, cancellationToken);

        if (session == null || !session.IsValid())
        {
            return new ValidateSessionResponse { Valid = false };
        }

        return new ValidateSessionResponse
        {
            Valid = true,
            UserId = session.UserId.ToString(),
            ExpiresAt = session.ExpiresAt
        };
    }
}
