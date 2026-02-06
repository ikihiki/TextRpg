using CoreBackend.Domain.Scenarios;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.CreateScenario;

/// <summary>
/// シナリオ作成のユースケース
/// </summary>
public class CreateScenario : IUseCase<CreateScenarioRequest, CreateScenarioResponse>
{
    private readonly IScenarioRepository _scenarioRepository;

    public CreateScenario(IScenarioRepository scenarioRepository)
    {
        _scenarioRepository = scenarioRepository;
    }

    public CreateScenarioResponse Execute(CreateScenarioRequest request)
    {
        // 同期的に非同期メソッドを呼び出す（IUseCaseインターフェースに合わせるため）
        return ExecuteAsync(request).GetAwaiter().GetResult();
    }

    private async Task<CreateScenarioResponse> ExecuteAsync(CreateScenarioRequest request)
    {
        // シナリオを作成（Draft状態で作成される）
        var scenario = Scenario.Create(request.Title, request.Summary, request.UserId);

        // リポジトリに保存
        await _scenarioRepository.AddAsync(scenario);

        return new CreateScenarioResponse
        {
            ScenarioId = scenario.Id.ToString(),
            CreatedAt = scenario.CreatedAt
        };
    }
}
