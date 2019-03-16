using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.ExosuitTechnology
{
    class RocketBoots : ComplexRecipe
    {
        public RocketBoots()
        {
            Name = "Rocket Boots";
            Type = "Jetpack Module";
            Ingredients.Add(IngredientType.TRITIUM, 100);
            ChildRecipes.Add(new SaltRefractor(), 1);
        }
    }
}
