using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CylindricalRoom : Recipe
    {
        public CylindricalRoom()
        {
            Name = "Cylindrical Room";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
