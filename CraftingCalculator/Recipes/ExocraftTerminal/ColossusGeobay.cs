using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTerminal
{
    class ColossusGeobay : ComplexRecipe
    {
        public ColossusGeobay()
        {
            Name = "Colossus Geobay";
            Type = "Exocraft Geobay";
            Ingredients.Add(IngredientType.PARAFFINIUM, 100);
            ChildRecipes.Add(new MetalPlating(), 5);
            ChildRecipes.Add(new IonBattery(), 4);
        }
    }
}
