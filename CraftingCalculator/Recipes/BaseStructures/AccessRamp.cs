using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
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
