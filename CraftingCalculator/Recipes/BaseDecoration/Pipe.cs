using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Pipe : Recipe
    {
        public Pipe()
        {
            Name = "Pipe";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
