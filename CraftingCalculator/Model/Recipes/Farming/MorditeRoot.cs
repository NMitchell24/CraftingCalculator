using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class MorditeRoot : Recipe
    {
        public MorditeRoot()
        {
            Name = "Mordite Root";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.MORDITE, 40);
        }
    }
}
