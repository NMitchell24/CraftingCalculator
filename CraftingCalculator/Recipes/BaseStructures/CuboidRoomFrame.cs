using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CuboidRoomFrame : Recipe
    {
        public CuboidRoomFrame()
        {
            Name = "Cuboid Room Frame";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
