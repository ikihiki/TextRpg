namespace AIOrchestrator.Domain.Narrative;

/// <summary>
/// 物語生成コンテキスト
/// </summary>
public class NarrativeContext
{
    public string SessionId { get; set; } = string.Empty;
    public string CurrentScene { get; set; } = string.Empty;
    public List<string> RecentHistory { get; set; } = new();
    public Dictionary<string, string> CharacterStates { get; set; } = new();

    // TODO: Implement narrative context logic
}
