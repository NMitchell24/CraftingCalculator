using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class PavingDecoration : Recipe
    {
        public PavingDecoration()
        {
            Name = "Paving (Decoration)";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 25);
        }
    }
}
