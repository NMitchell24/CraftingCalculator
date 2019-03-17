using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Sphere : Recipe
    {
        public Sphere()
        {
            Name = "Sphere";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
