using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class LivingGlass : ComplexRecipe
    {
        public LivingGlass()
        {
            Name = "Living Glass";
            Type = "Trade Item";
            ChildRecipes.Add(new Lubricant(), 1);
            ChildRecipes.Add(new Glass(), 5);
        }
    }
}
