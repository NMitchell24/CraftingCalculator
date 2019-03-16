using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class Glass : Recipe
    {
        public Glass()
        {
            Name = "Glass";
            Type = "Trade Item";
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 50);
        }
    }
}
