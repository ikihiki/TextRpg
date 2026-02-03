namespace CoreBackend.Domains.Turns;

/// <summary>
/// ターンエンティティ
/// </summary>
public class Turn
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public int TurnNumber { get; private set; }
    public TurnLog Log { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }

    // TODO: Implement turn logic
}
