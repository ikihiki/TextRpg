namespace RulesEngine.Domains.Dice;

/// <summary>
/// ダイスロール結果
/// </summary>
public class DiceRoll
{
    public int[] Results { get; set; } = Array.Empty<int>();
    public int Total { get; set; }
    public string Expression { get; set; } = string.Empty;

    // TODO: Implement dice roll logic
}
