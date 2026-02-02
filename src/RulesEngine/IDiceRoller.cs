namespace TextRpg.RulesEngine;

/// <summary>
/// Interface for dice rolling operations.
/// </summary>
public interface IDiceRoller
{
    /// <summary>
    /// Rolls a single die with the specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <returns>The result of the roll.</returns>
    int Roll(int sides);

    /// <summary>
    /// Rolls multiple dice with the specified number of sides.
    /// </summary>
    /// <param name="count">The number of dice to roll.</param>
    /// <param name="sides">The number of sides on each die.</param>
    /// <returns>An array of roll results.</returns>
    int[] Roll(int count, int sides);
}
