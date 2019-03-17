using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteRoofPanel : Recipe
    {
        public ConcreteRoofPanel()
        {
            Name = "Concrete Roof Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 25);
        }
    }
}
