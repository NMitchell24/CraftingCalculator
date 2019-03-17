using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
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
