using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class LShapedCorridor : Recipe
    {
        public LShapedCorridor()
        {
            Name = "L-Shaped Corridor";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
