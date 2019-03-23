using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class Aronium : Recipe
    {
        public Aronium()
        {
            Name = "Aronium";
            Type = RecipeFilterLabels.AlloyMetal;
            Ingredients.Add(IngredientType.PARAFFINIUM, 50);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
