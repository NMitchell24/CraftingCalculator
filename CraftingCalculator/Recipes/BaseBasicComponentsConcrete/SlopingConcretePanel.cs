using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class SlopingConcretePanel : Recipe
    {
        public SlopingConcretePanel()
        {
            Name = "Sloping Concrete Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
