using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class CuboidRoofCap : Recipe
    {
        public CuboidRoofCap()
        {
            Name = "Cuboid Roof Cap";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
