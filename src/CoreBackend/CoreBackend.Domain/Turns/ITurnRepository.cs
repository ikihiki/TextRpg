namespace CoreBackend.Domain.Turns;

/// <summary>
/// ターンリポジトリのインターフェース
/// </summary>
public interface ITurnRepository
{
    Task<Turn?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Turn>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Turn?> GetLatestTurnAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task AddAsync(Turn turn, CancellationToken cancellationToken = default);
    Task DeleteAfterTurnAsync(Guid sessionId, int turnNumber, CancellationToken cancellationToken = default);
}
