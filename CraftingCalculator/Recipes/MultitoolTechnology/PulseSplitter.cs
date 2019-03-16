using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class PulseSplitter : Recipe
    {
        public PulseSplitter()
        {
            Name = "Pulse Splitter";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            Ingredients.Add(IngredientType.DEUTERIUM, 200);
        }
    }
}
