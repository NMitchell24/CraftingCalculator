using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftSignalBoosterSigma : ComplexRecipe
    {
        public ExocraftSignalBoosterSigma()
        {
            Name = "Exocraft Signal Booster Upgrade Sigma";
            Type = "Exocraft Scanner Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            ChildRecipes.Add(new IonBattery(), 2);
            ChildRecipes.Add(new Microprocessor(), 2);
        }
    }
}
