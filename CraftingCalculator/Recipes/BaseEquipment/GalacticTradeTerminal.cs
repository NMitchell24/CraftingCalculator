using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class GalacticTradeTerminal : ComplexRecipe
    {
        public GalacticTradeTerminal()
        {
            Name = "Galactic Trade Terminal";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
            ChildRecipes.Add(new Microprocessor(), 3);
        }
    }
}
