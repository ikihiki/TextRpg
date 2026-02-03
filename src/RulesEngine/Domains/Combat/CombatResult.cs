namespace RulesEngine.Domains.Combat;

/// <summary>
/// 戦闘結果
/// </summary>
public class CombatResult
{
    public bool IsVictory { get; set; }
    public List<CombatEvent> Events { get; set; } = new();
    public Dictionary<string, int> DamageDealt { get; set; } = new();

    // TODO: Implement combat result logic
}

public class CombatEvent
{
    public string Description { get; set; } = string.Empty;
    public string ActorId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
}
