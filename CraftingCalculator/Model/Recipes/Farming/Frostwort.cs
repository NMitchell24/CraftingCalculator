using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class Frostwort : Recipe
    {
        public Frostwort()
        {
            Name = "Frostwort";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 50);
            Ingredients.Add(IngredientType.DIOXITE, 25);
        }
    }
}
