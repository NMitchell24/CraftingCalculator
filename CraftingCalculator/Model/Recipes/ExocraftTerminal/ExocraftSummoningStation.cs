using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
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
