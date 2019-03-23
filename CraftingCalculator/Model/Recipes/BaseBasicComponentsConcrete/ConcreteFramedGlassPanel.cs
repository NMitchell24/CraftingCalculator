using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFramedGlassPanel : Recipe
    {
        public ConcreteFramedGlassPanel()
        {
            Name = "Concrete-Framed Glass Panel";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
