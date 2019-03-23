using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Sphere : Recipe
    {
        public Sphere()
        {
            Name = "Sphere";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
