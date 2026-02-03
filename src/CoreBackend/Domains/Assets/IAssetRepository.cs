namespace CoreBackend.Domains.Assets;

/// <summary>
/// アセットリポジトリのインターフェース
/// </summary>
public interface IAssetRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
