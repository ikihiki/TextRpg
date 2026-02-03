namespace RulesEngine.Domains.Combat;

/// <summary>
/// 戦闘参加者
/// </summary>
public class Combatant
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public Dictionary<string, int> Stats { get; set; } = new();

    // TODO: Implement combatant logic
}
