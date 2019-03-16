using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class StarshipLaunchFuel : ComplexRecipe
    {
        public StarshipLaunchFuel()
        {
            Name = "Starship Launch Fuel";
            Type = "Consumable (Starship)";
            Ingredients.Add(IngredientType.DIHYDROGEN, 40);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
