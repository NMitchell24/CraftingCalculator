using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CuboidRoomFoundationStrutQuad : Recipe
    {
        public CuboidRoomFoundationStrutQuad()
        {
            Name = "Cuboid Room Foundation Strut Quad";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
