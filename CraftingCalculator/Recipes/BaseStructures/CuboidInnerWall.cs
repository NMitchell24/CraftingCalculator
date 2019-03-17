using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CuboidInnerWall : Recipe
    {
        public CuboidInnerWall()
        {
            Name = "Cuboid Inner Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
