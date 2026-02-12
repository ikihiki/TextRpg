using VerticalSliceArchitecture.Core;

namespace Story.Features.ScenarioStart;

public class StartScenario : IUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public ValueTask<StartScenarioResponse> ExecuteAsync(StartScenarioRequest request, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(new StartScenarioResponse
        {
            Success = true,
            Message = $"Scenario {request.ScenarioId} started successfully"
        });
    }
}
