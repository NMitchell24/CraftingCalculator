using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CraftingCalculator.Infrastructure.Data.Configurations;

public class DatasetConfiguration : IEntityTypeConfiguration<Dataset>
{
    public void Configure(EntityTypeBuilder<Dataset> builder)
    {
        builder.ToTable("Datasets");
        builder.HasKey(dataset => dataset.Id);
        builder.Property(dataset => dataset.Name).IsRequired();
        builder.HasIndex(dataset => dataset.Name);
    }
}
