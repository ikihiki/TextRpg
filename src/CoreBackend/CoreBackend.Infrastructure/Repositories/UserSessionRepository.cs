using CoreBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Infrastructure.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly CoreBackendDbContext _context;

    public UserSessionRepository(CoreBackendDbContext context)
    {
        _context = context;
    }

    public async Task<UserSession?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .FirstOrDefaultAsync(s => s.Token == token, cancellationToken);
    }

    public async Task<IEnumerable<UserSession>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        await _context.UserSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        _context.UserSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
