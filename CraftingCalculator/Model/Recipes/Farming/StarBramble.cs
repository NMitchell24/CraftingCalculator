using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class StarBramble : Recipe
    {
        public StarBramble()
        {
            Name = "Star Bramble";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.STAR_BULB, 50);
            Ingredients.Add(IngredientType.PARAFFINIUM, 25);
        }
    }
}
