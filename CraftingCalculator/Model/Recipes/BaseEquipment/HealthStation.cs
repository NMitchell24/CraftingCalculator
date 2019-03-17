using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.BaseEquipment
{
    class HealthStation : ComplexRecipe
    {
        public HealthStation()
        {
            Name = "Health Station";
            Type = "Base Equipment";
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            Ingredients.Add(IngredientType.URANIUM, 50);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
