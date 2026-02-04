using CoreBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CoreBackend.Infrastructure;

public class CoreBackendDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public CoreBackendDbContext(DbContextOptions<CoreBackendDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value));

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.IconUrl)
                .HasMaxLength(500);

            entity.Property(u => u.Bio)
                .HasMaxLength(1000);

            entity.Property(u => u.Language)
                .HasMaxLength(10)
                .HasDefaultValue("ja");

            entity.OwnsOne(u => u.NotificationSettings, ns =>
            {
                ns.Property(n => n.NoteUpdates).HasDefaultValue(true);
                ns.Property(n => n.SessionReminders).HasDefaultValue(true);
                ns.Property(n => n.Marketing).HasDefaultValue(false);
            });

            entity.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            entity.HasQueryFilter(u => !u.IsDeleted);
        });

        // UserSession configuration
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.UserId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.From(value));

            entity.Property(s => s.Token)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(s => s.Token)
                .IsUnique();

            entity.HasIndex(s => s.UserId);
        });
    }
}
