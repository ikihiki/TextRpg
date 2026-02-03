namespace RulesEngine.Domains.Combat;

/// <summary>
/// 戦闘解決器のインターフェース
/// 数値・確率・乱数はルールエンジンが決定論的に処理
/// </summary>
public interface ICombatResolver
{
    CombatResult ResolveCombat(IEnumerable<Combatant> attackers, IEnumerable<Combatant> defenders);
}
