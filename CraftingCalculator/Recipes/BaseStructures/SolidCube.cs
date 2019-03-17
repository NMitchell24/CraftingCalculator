using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class SolidCube : Recipe
    {
        public SolidCube()
        {
            Name = "Solid Cube";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
