using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class LightTable : Recipe
    {
        public LightTable()
        {
            Name = "Light Table";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 10);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
