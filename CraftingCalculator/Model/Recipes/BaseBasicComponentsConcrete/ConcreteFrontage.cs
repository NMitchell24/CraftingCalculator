using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFrontage : Recipe
    {
        public ConcreteFrontage()
        {
            Name = "Concrete Frontage";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
