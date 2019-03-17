using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class CylindricalRoomFrame : Recipe
    {
        public CylindricalRoomFrame()
        {
            Name = "Cylindrical Room Frame";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 200);
        }
    }
}
