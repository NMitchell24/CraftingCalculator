using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CuboidRoomFoundationStrut : Recipe
    {
        public CuboidRoomFoundationStrut()
        {
            Name = "Cuboid Room Foundation Strut";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 80);
        }
    }
}
