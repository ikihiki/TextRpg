using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.CreateSession;

public class CreateSession : IUseCase<CreateSessionRequest, CreateSessionResponse>
{
    public CreateSessionResponse Execute(CreateSessionRequest request)
    {
        var sessionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // TODO: Persist session to database via repository
        return new CreateSessionResponse
        {
            SessionId = sessionId,
            CreatedAt = createdAt
        };
    }
}
