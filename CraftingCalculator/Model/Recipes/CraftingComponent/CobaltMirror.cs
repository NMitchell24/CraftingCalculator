using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class CobaltMirror : Recipe
    {
        public CobaltMirror()
        {
            Name = "Cobalt Mirror";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
