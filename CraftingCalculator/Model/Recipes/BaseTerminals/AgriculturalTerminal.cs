using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseTerminals
{
    class AgriculturalTerminal : Recipe
    {
        public AgriculturalTerminal()
        {
            Name = "Agricultural Terminal";
            Type = "Base Terminal";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 25);
        }
    }
}
