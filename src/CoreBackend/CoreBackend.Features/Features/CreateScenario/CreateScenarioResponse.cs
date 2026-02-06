namespace CoreBackend.Features.CreateScenario;

/// <summary>
/// シナリオ作成レスポンス
/// </summary>
public class CreateScenarioResponse
{
    /// <summary>
    /// 作成されたシナリオのID
    /// </summary>
    public required string ScenarioId { get; init; }

    /// <summary>
    /// 作成日時
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
