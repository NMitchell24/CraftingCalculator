using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Cube : Recipe
    {
        public Cube()
        {
            Name = "Cube";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
