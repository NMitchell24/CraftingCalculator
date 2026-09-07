using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired();
        builder.Property(i => i.Description).IsRequired();
        builder.HasIndex(i => i.Name);
    }
}
