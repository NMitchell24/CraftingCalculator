namespace CraftingCalculator.Domain.Entities;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Cost { get; set; }

    public List<RecipeIngredient> RecipeIngredients { get; set; } = [];
}
