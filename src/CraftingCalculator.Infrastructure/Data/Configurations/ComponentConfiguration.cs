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

        //SetNull rather than Cascade, matching the same relationship on Blueprint: deleting a category
        //is a filing change, not a reason to delete what was filed under it.
        builder.HasOne(component => component.Category)
            .WithMany(category => category.Components)
            .HasForeignKey(component => component.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
