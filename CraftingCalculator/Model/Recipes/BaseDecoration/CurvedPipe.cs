using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class CurvedPipe : Recipe
    {
        public CurvedPipe()
        {
            Name = "Curved Pipe";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
