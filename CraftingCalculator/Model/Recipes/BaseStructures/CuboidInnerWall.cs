using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidInnerWall : Recipe
    {
        public CuboidInnerWall()
        {
            Name = "Cuboid Inner Wall";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
