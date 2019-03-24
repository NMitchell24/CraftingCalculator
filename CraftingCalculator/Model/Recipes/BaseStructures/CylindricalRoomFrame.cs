using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CylindricalRoomFrame : Recipe
    {
        public CylindricalRoomFrame()
        {
            Name = "Cylindrical Room Frame";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 200);
        }
    }
}
