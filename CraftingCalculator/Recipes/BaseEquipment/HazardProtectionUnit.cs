using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class HazardProtectionUnit : ComplexRecipe
    {
        public HazardProtectionUnit()
        {
            Name = "Hazard Protection Unit";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            Ingredients.Add(IngredientType.PHOSPHORUS, 50);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
