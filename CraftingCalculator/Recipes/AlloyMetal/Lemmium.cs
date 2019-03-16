using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AlloyMetal
{
    class Lemmium : Recipe
    {
        public Lemmium()
        {
            Name = "Lemmium";
            Type = "Alloy Metal";
            Ingredients.Add(IngredientType.URANIUM, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
