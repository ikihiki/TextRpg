namespace CoreBackend.Domain.Scenarios;

/// <summary>
/// シナリオの状態を表す列挙型
/// </summary>
public enum ScenarioStatus
{
    /// <summary>
    /// 下書き状態
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 公開済み
    /// </summary>
    Published = 1,

    /// <summary>
    /// アーカイブ済み
    /// </summary>
    Archived = 2
}
