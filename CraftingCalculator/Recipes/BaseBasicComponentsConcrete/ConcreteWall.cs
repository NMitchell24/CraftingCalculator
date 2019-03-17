using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteWall : Recipe
    {
        public ConcreteWall()
        {
            Name = "Concrete Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
        }
    }
}
