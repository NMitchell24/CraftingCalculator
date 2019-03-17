using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTechnology
{
    class ExocraftSignalBoosterTau : ComplexRecipe
    {
        public ExocraftSignalBoosterTau()
        {
            Name = "Exocraft Signal Booster Upgrade Tau";
            Type = "Exocraft Scanner Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 150);
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
