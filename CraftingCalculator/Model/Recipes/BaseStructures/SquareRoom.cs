using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class SquareRoom : Recipe
    {
        public SquareRoom()
        {
            Name = "Square Room";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
