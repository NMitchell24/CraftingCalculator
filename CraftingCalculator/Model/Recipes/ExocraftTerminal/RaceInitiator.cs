using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
{
    class RaceInitiator : ComplexRecipe
    {
        public RaceInitiator()
        {
            Name = "Race Initiator";
            Type = "Exocraft Race Module";
            ChildRecipes.Add(new MetalPlating(), 5);
            ChildRecipes.Add(new Microprocessor(), 2);
            ChildRecipes.Add(new IonBattery(), 1);
        }
    }
}
