using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.BaseEquipment
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
