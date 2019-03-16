using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AlloyMetal
{
    class Aronium : Recipe
    {
        public Aronium()
        {
            Name = "Aronium";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.PARAFFINIUM, 50);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
