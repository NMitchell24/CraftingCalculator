using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
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
