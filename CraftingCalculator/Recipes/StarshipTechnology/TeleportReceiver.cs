using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class TeleportReceiver : ComplexRecipe
    {
        public TeleportReceiver()
        {
            Name = "Teleport Receiver";
            Type = "Starship Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 3);
            ChildRecipes.Add(new Antimatter(), 1);
        }
    }
}
