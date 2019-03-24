using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftSignalBoosterTau : ComplexRecipe
    {
        public ExocraftSignalBoosterTau()
        {
            Name = "Exocraft Signal Booster Upgrade Tau";
            Type = RecipeFilterLabels.ExocraftTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 150);
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new IonBattery(), 2);
        }
    }
}
