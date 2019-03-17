using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class CylindricalRoom : Recipe
    {
        public CylindricalRoom()
        {
            Name = "Cylindrical Room";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
