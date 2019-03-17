using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteFloorPanel : Recipe
    {
        public ConcreteFloorPanel()
        {
            Name = "Concrete Floor Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
