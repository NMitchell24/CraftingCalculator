using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenDoorFrame : Recipe
    {
        public WoodenDoorFrame()
        {
            Name = "Wooden Door Frame";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
