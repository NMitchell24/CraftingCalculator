using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFramedGlassPanel : Recipe
    {
        public ConcreteFramedGlassPanel()
        {
            Name = "Concrete-Framed Glass Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
