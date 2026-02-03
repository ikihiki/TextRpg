namespace CoreBackend.Domains.Sessions;

/// <summary>
/// セッションリポジトリのインターフェース
/// </summary>
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Session session, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session session, CancellationToken cancellationToken = default);
    Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default);
}
