using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintConfiguration : IEntityTypeConfiguration<Blueprint>
{
    public void Configure(EntityTypeBuilder<Blueprint> builder)
    {
        builder.ToTable("Blueprints");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.Description).IsRequired();
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.CategoryId);

        builder.HasOne(r => r.Category)
            .WithMany(f => f.Blueprints)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
