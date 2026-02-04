using CoreBackend.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Service.Infrastructure.Persistence;

/// <summary>
/// アプリケーションのデータベースコンテキスト
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Scenario> Scenarios => Set<Scenario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
