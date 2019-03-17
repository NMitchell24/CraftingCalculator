using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTerminal
{
    class ExocraftSummoningStation : ComplexRecipe
    {
        public ExocraftSummoningStation()
        {
            Name = "Exocraft Summoning Station";
            Type = "Exocraft Summoning Station";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 100);
            ChildRecipes.Add(new IonBattery(), 4);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
