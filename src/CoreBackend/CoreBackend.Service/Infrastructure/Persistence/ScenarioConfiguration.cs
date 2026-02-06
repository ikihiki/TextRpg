using CoreBackend.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreBackend.Service.Infrastructure.Persistence;

/// <summary>
/// Scenarioエンティティの設定
/// </summary>
public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("Scenarios");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => ScenarioId.From(value))
            .IsRequired();

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Summary)
            .HasMaxLength(2000);

        builder.Property(s => s.OwnerId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // インデックス
        builder.HasIndex(s => s.OwnerId);
        builder.HasIndex(s => s.Status);
    }
}
