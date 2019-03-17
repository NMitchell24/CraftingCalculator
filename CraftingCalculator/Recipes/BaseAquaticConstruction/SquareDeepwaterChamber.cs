using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseAquaticConstruction
{
    class SquareDeepwaterChamber : Recipe
    {
        public SquareDeepwaterChamber()
        {
            Name = "Square Deepwater Chamber";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
