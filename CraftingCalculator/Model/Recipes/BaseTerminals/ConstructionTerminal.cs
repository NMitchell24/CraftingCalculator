using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseTerminals
{
    class ConstructionTerminal : Recipe
    {
        public ConstructionTerminal()
        {
            Name = "Construction Terminal";
            Type = "Base Terminal";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 25);
        }
    }
}
