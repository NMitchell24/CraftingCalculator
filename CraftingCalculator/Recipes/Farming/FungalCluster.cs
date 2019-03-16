using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class FungalCluster : Recipe
    {
        public FungalCluster()
        {
            Name = "Fungal Cluster";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.FUNGAL_MOULD, 50);
            Ingredients.Add(IngredientType.AMMONIA, 25);
        }
    }
}
