using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SimpleDesk : Recipe
    {
        public SimpleDesk()
        {
            Name = "Simple Desk";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
