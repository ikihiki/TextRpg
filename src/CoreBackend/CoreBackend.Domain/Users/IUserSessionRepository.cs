namespace CoreBackend.Domain.Users;

/// <summary>
/// ユーザーセッションリポジトリのインターフェース
/// </summary>
public interface IUserSessionRepository
{
    Task<UserSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
}
