namespace CraftingCalculator.Domain.Entities;

public class Favorite
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<FavoriteRecipe> FavoriteRecipes { get; set; } = [];
}
