
namespace CraftingCalculator.Model.Recipes
{
    class RecipeMultiplier
    {
        public Recipe Recipe { get; set; }
        public int Quantity { get; set; }

        public RecipeMultiplier(Recipe recipe, int quantity)
        {
            Recipe = recipe;
            Quantity = quantity;
        }
    }
}
