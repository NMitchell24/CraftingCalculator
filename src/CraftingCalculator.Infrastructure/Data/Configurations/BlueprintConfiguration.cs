using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class BlueprintConfiguration : IEntityTypeConfiguration<Blueprint>
{
    public void Configure(EntityTypeBuilder<Blueprint> builder)
    {
        builder.ToTable("Blueprints");
        builder.HasKey(blueprint => blueprint.Id);
        builder.Property(blueprint => blueprint.Name).IsRequired();
        builder.Property(blueprint => blueprint.Description).IsRequired();
        //A store default so the column added by AddBlueprintYield backfills rows written before yield existed.
        builder.Property(blueprint => blueprint.Yield).HasDefaultValue(1L);
        builder.HasIndex(blueprint => blueprint.Name);
        builder.HasIndex(blueprint => blueprint.CategoryId);

        builder.HasOne(blueprint => blueprint.Category)
            .WithMany(category => category.Blueprints)
            .HasForeignKey(blueprint => blueprint.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
