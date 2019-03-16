using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.ExosuitTechnology
{
    class OxygenRerouter : ComplexRecipe
    {
        public OxygenRerouter()
        {
            Name = "Oxygen Rerouter";
            Type = "Life Support Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 6);
            ChildRecipes.Add(new SaltRefractor(), 1);
        }
    }
}
