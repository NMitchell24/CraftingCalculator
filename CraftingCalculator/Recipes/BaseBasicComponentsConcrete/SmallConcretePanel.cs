using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class SmallConcretePanel : Recipe
    {
        public SmallConcretePanel()
        {
            Name = "Small Concrete Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
