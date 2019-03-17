using CraftingCalculator.Recipes.CraftingComponent;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class LandingPad : ComplexRecipe
    {
        public LandingPad()
        {
            Name = "Landing Pad";
            Type = "Base Equipment";
            ChildRecipes.Add(new MetalPlating(), 10);
            ChildRecipes.Add(new IonBattery(), 2);
            ChildRecipes.Add(new Microprocessor(), 2);
        }
    }
}
