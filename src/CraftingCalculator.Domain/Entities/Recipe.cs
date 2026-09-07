namespace CraftingCalculator.Domain.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Value { get; set; }

    public int? FilterId { get; set; }
    public RecipeFilter? Filter { get; set; }

    public List<RecipeIngredient> Ingredients { get; set; } = [];

    /// <summary>Component links where this recipe is the parent.</summary>
    public List<RecipeChild> Children { get; set; } = [];

    /// <summary>Component links where this recipe is used as a child of another recipe.</summary>
    public List<RecipeChild> ParentLinks { get; set; } = [];

    public List<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
}
