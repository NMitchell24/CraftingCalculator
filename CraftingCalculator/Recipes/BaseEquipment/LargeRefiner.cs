using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.BaseEquipment
{
    class LargeRefiner : ComplexRecipe
    {
        public LargeRefiner()
        {
            Name = "Large Refiner";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 200);
            Ingredients.Add(IngredientType.SODIUM, 100);
            ChildRecipes.Add(new Microprocessor(), 5);
        }
    }
}
