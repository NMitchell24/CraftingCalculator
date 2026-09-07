namespace CraftingCalculator.Domain.Entities;

public class Component
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Cost { get; set; }

    public List<RecipeComponent> RecipeComponents { get; set; } = [];
}
