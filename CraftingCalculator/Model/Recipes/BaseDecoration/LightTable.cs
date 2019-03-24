using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class LightTable : Recipe
    {
        public LightTable()
        {
            Name = "Light Table";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.SODIUM, 10);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
