using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
