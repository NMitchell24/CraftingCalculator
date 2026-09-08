namespace CraftingCalculator.Domain.Entities;

public class Blueprint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Value { get; set; }

    /// <summary>How many items one craft of this blueprint produces.</summary>
    public long Yield { get; set; } = 1;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<BlueprintComponent> Components { get; set; } = [];

    /// <summary>Component links where this blueprint is the parent.</summary>
    public List<BlueprintChild> Children { get; set; } = [];

    /// <summary>Component links where this blueprint is used as a child of another blueprint.</summary>
    public List<BlueprintChild> ParentLinks { get; set; } = [];

    public List<FavoriteBlueprint> FavoriteBlueprints { get; set; } = [];
}
