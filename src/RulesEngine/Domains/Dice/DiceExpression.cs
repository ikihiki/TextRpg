namespace RulesEngine.Domains.Dice;

/// <summary>
/// ダイス式（例: 2d6+3）
/// </summary>
public class DiceExpression
{
    public int DiceCount { get; set; }
    public int DiceSides { get; set; }
    public int Modifier { get; set; }

    public static DiceExpression Parse(string expression)
    {
        // TODO: Implement expression parsing (e.g., "2d6+3")
        return new DiceExpression();
    }

    public override string ToString() => $"{DiceCount}d{DiceSides}{(Modifier >= 0 ? "+" : "")}{Modifier}";
}
