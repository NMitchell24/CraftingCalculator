using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class Scanner : Recipe
    {
        public Scanner()
        {
            Name = "Scanner";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.FERRITE_DUST, 150);
        }
    }
}
