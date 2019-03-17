using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class AccessRamp : Recipe
    {
        public AccessRamp()
        {
            Name = "Access Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
            Ingredients.Add(IngredientType.CARBON, 50);
        }
    }
}
