using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class WallUnit : Recipe
    {
        public WallUnit()
        {
            Name = "Wall Unit";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
