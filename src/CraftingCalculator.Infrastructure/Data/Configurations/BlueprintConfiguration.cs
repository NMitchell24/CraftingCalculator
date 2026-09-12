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
        //Declares the store default, which is what makes AddBlueprintYield scaffold its AddColumn with
        //defaultValue: 1L instead of the CLR default; that operation is what fills the column for rows
        //written before yield existed.
        builder.Property(blueprint => blueprint.Yield).HasDefaultValue(1L);
        //Stored as ticks (INTEGER) rather than the provider's default TEXT so the column round-trips
        //losslessly and stays cheap to compare. No HasDefaultValue here, unlike Yield above: the
        //default is TimeSpan.Zero, which is already what SQLite backfills into a NOT NULL INTEGER.
        builder.Property(blueprint => blueprint.ProductionTime).HasConversion<long>();
        builder.HasIndex(blueprint => blueprint.Name);
        builder.HasIndex(blueprint => blueprint.CategoryId);

        builder.HasIndex(blueprint => blueprint.DatasetId);

        //Cascade: a dataset is the container for its records, so deleting one takes its
        //contents with it.
        builder.HasOne(blueprint => blueprint.Dataset)
            .WithMany(dataset => dataset.Blueprints)
            .HasForeignKey(blueprint => blueprint.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(blueprint => blueprint.Category)
            .WithMany(category => category.Blueprints)
            .HasForeignKey(blueprint => blueprint.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
