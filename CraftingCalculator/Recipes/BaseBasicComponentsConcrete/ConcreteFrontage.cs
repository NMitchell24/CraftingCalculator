using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFrontage : Recipe
    {
        public ConcreteFrontage()
        {
            Name = "Concrete Frontage";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
