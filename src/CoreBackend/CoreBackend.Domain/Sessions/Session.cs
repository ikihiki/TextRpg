namespace CoreBackend.Domain.Sessions;

/// <summary>
/// セッションの集約ルートエンティティ
/// </summary>
public class Session
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public SessionState State { get; private set; } = new();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // TODO: Implement session logic
}
