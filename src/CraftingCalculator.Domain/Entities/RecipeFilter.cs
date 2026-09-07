namespace CraftingCalculator.Domain.Entities;

public class RecipeFilter
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Recipe> Recipes { get; set; } = [];
}
