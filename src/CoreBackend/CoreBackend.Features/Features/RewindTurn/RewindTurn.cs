using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.RewindTurn;

public class RewindTurn : IUseCase<RewindTurnRequest, RewindTurnResponse>
{
    public RewindTurnResponse Execute(RewindTurnRequest request)
    {
        // TODO: Invalidate turns after the target turn in database via repository
        return new RewindTurnResponse
        {
            Success = true,
            InvalidatedCount = 0
        };
    }
}
