using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class SmallConcretePanel : Recipe
    {
        public SmallConcretePanel()
        {
            Name = "Small Concrete Panel";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
