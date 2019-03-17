using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTerminal
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
