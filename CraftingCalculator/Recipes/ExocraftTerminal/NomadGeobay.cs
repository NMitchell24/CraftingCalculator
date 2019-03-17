using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTerminal
{
    class NomadGeobay : ComplexRecipe
    {
        public NomadGeobay()
        {
            Name = "Nomad Geobay";
            Type = "Exocraft Geobay";
            Ingredients.Add(IngredientType.PARAFFINIUM, 100);
            ChildRecipes.Add(new MetalPlating(), 5);
            ChildRecipes.Add(new IonBattery(), 4);
        }
    }
}
