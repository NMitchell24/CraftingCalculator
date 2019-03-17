using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class MonitorStation : Recipe
    {
        public MonitorStation()
        {
            Name = "Monitor Station";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
