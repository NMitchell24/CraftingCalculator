using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteHalfRamp : Recipe
    {
        public ConcreteHalfRamp()
        {
            Name = "Concrete Half Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
        }
    }
}
