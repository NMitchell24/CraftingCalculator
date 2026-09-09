namespace CraftingCalculator.Domain.Entities;

public class Component
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Cost { get; set; }

    /// <summary>How long one of this component takes to gather or produce. Zero is instant.</summary>
    public TimeSpan ProductionTime { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<BlueprintComponent> BlueprintComponents { get; set; } = [];
}
