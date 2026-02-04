namespace CoreBackend.Features.CreateScenario;

/// <summary>
/// シナリオ作成リクエスト
/// </summary>
public class CreateScenarioRequest
{
    /// <summary>
    /// シナリオタイトル（必須）
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// シナリオ概要（任意）
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// 所有者のユーザーID
    /// </summary>
    public required string UserId { get; init; }
}
