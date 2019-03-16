using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class StarBramble : Recipe
    {
        public StarBramble()
        {
            Name = "Star Bramble";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.STAR_BULB, 50);
            Ingredients.Add(IngredientType.PARAFFINIUM, 25);
        }
    }
}
