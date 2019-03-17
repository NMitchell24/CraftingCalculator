using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class Herox : Recipe
    {
        public Herox()
        {
            Name = "Herox";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.AMMONIA, 50);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
        }
    }
}
