using VerticalSliceArchitecture.Core;

namespace Battle.Features.ResolveBattle;

public class ResolveBattle : IUseCase<ResolveBattleRequest, ResolveBattleResponse>
{
    public ResolveBattleResponse Execute(ResolveBattleRequest request)
    {
        return new ResolveBattleResponse
        {
            Victory = true,
            Result = $"Battle {request.BattleId} resolved"
        };
    }
}
