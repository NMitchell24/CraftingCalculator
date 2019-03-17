using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseTerminals
{
    class WeaponsTerminal : Recipe
    {
        public WeaponsTerminal()
        {
            Name = "Weapons Terminal";
            Type = "Base Terminal";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.PUGNEUM, 25);
        }
    }
}
