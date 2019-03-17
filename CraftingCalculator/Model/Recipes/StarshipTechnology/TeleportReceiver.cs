using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
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
