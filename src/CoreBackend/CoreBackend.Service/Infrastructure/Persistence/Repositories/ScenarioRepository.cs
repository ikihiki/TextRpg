using CoreBackend.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Service.Infrastructure.Persistence.Repositories;

/// <summary>
/// シナリオリポジトリの実装
/// </summary>
public class ScenarioRepository : IScenarioRepository
{
    private readonly AppDbContext _context;

    public ScenarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Scenario?> GetByIdAsync(ScenarioId id, CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Scenario>> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .Where(s => s.OwnerId == ownerId)
            .OrderByDescending(s => s.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        await _context.Scenarios.AddAsync(scenario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        _context.Scenarios.Update(scenario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScenarioId id, CancellationToken cancellationToken = default)
    {
        var scenario = await GetByIdAsync(id, cancellationToken);
        if (scenario != null)
        {
            _context.Scenarios.Remove(scenario);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
