using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
