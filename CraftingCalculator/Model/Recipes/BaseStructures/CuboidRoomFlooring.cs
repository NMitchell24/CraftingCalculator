using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidRoomFlooring : Recipe
    {
        public CuboidRoomFlooring()
        {
            Name = "Cuboid Room Flooring";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
