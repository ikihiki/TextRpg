namespace CoreBackend.Domains.Turns;

/// <summary>
/// ターンログエンティティ
/// </summary>
public class TurnLog
{
    public string NarrativeText { get; set; } = string.Empty;
    public string? PlayerAction { get; set; }
    public List<string> SystemEvents { get; set; } = new();

    // TODO: Implement turn log logic
}
