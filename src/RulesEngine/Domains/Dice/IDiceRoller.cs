namespace RulesEngine.Domains.Dice;

/// <summary>
/// ダイスローラーのインターフェース
/// 数値・確率・乱数はルールエンジンが決定論的に処理
/// </summary>
public interface IDiceRoller
{
    DiceRoll Roll(DiceExpression expression);
    DiceRoll Roll(string expression);
}
