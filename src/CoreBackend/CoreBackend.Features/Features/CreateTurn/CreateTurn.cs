using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.CreateTurn;

public class CreateTurn : IUseCase<CreateTurnRequest, CreateTurnResponse>
{
    public CreateTurnResponse Execute(CreateTurnRequest request)
    {
        var createdAt = DateTime.UtcNow;

        // TODO: Persist turn to database via repository
        return new CreateTurnResponse
        {
            TurnId = request.TurnNumber,
            CreatedAt = createdAt
        };
    }
}
