using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class Grantine : Recipe
    {
        public Grantine()
        {
            Name = "Grantine";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
            Ingredients.Add(IngredientType.DIOXITE, 50);
        }
    }
}
