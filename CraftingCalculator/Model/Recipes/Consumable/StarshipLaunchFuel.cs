using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class StarshipLaunchFuel : ComplexRecipe
    {
        public StarshipLaunchFuel()
        {
            Name = "Starship Launch Fuel";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.DIHYDROGEN, 40);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
