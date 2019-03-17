using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseAquaticConstruction
{
    class DeepwaterChamber : Recipe
    {
        public DeepwaterChamber()
        {
            Name = "Deepwater Chamber";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 350);
        }
    }
}
