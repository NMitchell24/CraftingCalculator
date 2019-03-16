using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class Scanner : Recipe
    {
        public Scanner()
        {
            Name = "Scanner";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.FERRITE_DUST, 150);
        }
    }
}
