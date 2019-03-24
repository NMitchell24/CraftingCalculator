using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFloorPanel : Recipe
    {
        public ConcreteFloorPanel()
        {
            Name = "Concrete Floor Panel";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
