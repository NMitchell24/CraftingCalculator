using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenDoorway : Recipe
    {
        public WoodenDoorway()
        {
            Name = "Wooden Doorway";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
