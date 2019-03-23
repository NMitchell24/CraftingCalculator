using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseAquaticConstruction
{
    class SquareDeepwaterChamber : Recipe
    {
        public SquareDeepwaterChamber()
        {
            Name = "Square Deepwater Chamber";
            Type = RecipeFilterLabels.BaseAquaticConstruction;
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
