using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.ToTable("Components");
        builder.HasKey(component => component.Id);
        builder.Property(component => component.Name).IsRequired();
        builder.Property(component => component.Description).IsRequired();
        //Ticks, for the reason given in BlueprintConfiguration.
        builder.Property(component => component.ProductionTime).HasConversion<long>();
        builder.HasIndex(component => component.Name);
        builder.HasIndex(component => component.CategoryId);

        //DatasetId first: every read is filtered to one dataset, and the list is ordered by name, so the index
        //serves both and SQLite skips the sort. It also serves as the Dataset foreign key's index.
        builder.HasIndex(component => new { component.DatasetId, component.Name });

        //Cascade: a dataset is the container for its records, so deleting one takes its
        //contents with it.
        builder.HasOne(component => component.Dataset)
            .WithMany(dataset => dataset.Components)
            .HasForeignKey(component => component.DatasetId)
            .OnDelete(DeleteBehavior.Cascade);

        //SetNull rather than Cascade, matching the same relationship on Blueprint: deleting a category
        //is a filing change, not a reason to delete what was filed under it.
        builder.HasOne(component => component.Category)
            .WithMany(category => category.Components)
            .HasForeignKey(component => component.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
