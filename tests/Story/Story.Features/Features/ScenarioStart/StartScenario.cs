using VerticalSliceArchitecture.Core;

namespace Story.Features.ScenarioStart;

public class StartScenario : IUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse
        {
            Success = true,
            Message = $"Scenario {request.ScenarioId} started successfully"
        };
    }
}
