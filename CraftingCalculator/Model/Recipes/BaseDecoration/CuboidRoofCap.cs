using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class CuboidRoofCap : Recipe
    {
        public CuboidRoofCap()
        {
            Name = "Cuboid Roof Cap";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
