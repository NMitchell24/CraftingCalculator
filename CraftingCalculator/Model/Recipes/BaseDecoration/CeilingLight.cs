using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class CeilingLight : Recipe
    {
        public CeilingLight()
        {
            Name = "Ceiling Light";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
