using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ShortConcreteWall : Recipe
    {
        public ShortConcreteWall()
        {
            Name = "Short Concrete Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
