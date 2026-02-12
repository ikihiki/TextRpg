using VerticalSliceArchitecture.Core;

namespace Battle.Features.ResolveBattle;

public class ResolveBattle : IUseCase<ResolveBattleRequest, ResolveBattleResponse>
{
    public ValueTask<ResolveBattleResponse> ExecuteAsync(ResolveBattleRequest request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(new ResolveBattleResponse
        {
            Victory = true,
            Result = $"Battle {request.BattleId} resolved"
        });
    }
}
