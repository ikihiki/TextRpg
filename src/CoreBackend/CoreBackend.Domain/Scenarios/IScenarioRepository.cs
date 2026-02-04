namespace CoreBackend.Domain.Scenarios;

/// <summary>
/// シナリオリポジトリのインターフェース
/// </summary>
public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(ScenarioId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Scenario>> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Scenario scenario, CancellationToken cancellationToken = default);
    Task UpdateAsync(Scenario scenario, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScenarioId id, CancellationToken cancellationToken = default);
}
