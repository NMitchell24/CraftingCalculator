using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class FungalCluster : Recipe
    {
        public FungalCluster()
        {
            Name = "Fungal Cluster";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.FUNGAL_MOULD, 50);
            Ingredients.Add(IngredientType.AMMONIA, 25);
        }
    }
}
