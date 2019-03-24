using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteHalfRamp : Recipe
    {
        public ConcreteHalfRamp()
        {
            Name = "Concrete Half Ramp";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
