using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class Echinocactus : Recipe
    {
        public Echinocactus()
        {
            Name = "Echinocactus";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.CACTUS_FLESH, 50);
            Ingredients.Add(IngredientType.PYRITE, 25);
        }
    }
}
