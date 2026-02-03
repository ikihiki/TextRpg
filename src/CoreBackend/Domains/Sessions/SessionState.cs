namespace CoreBackend.Domains.Sessions;

/// <summary>
/// セッションの状態エンティティ
/// </summary>
public class SessionState
{
    public int CurrentTurn { get; set; }
    public string? CurrentSceneId { get; set; }

    // Extensible state data
    public Dictionary<string, object> Ext { get; set; } = new();

    // TODO: Implement state management logic
}
