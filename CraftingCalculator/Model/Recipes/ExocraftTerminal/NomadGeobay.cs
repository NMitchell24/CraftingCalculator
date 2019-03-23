using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
{
    class NomadGeobay : ComplexRecipe
    {
        public NomadGeobay()
        {
            Name = "Nomad Geobay";
            Type = RecipeFilterLabels.ExocraftTerminals;
            Ingredients.Add(IngredientType.PARAFFINIUM, 100);
            ChildRecipes.Add(new MetalPlating(), 5);
            ChildRecipes.Add(new IonBattery(), 4);
        }
    }
}
