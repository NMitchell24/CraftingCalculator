using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class SolidCube : Recipe
    {
        public SolidCube()
        {
            Name = "Solid Cube";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
