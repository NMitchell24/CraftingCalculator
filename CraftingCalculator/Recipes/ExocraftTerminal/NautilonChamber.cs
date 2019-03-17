using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.ExocraftTerminal
{
    class NautilonChamber : ComplexRecipe
    {
        public NautilonChamber()
        {
            Name = "Nautilon Chamber";
            Type = "Exocraft Geobay";
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 4);
            Ingredients.Add(IngredientType.SALT, 100);
            ChildRecipes.Add(new MetalPlating(), 5);
        }
    }
}
