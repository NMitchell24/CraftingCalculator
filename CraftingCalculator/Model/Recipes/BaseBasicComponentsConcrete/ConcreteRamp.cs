using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteRamp : Recipe
    {
        public ConcreteRamp()
        {
            Name = "Concrete Ramp";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
