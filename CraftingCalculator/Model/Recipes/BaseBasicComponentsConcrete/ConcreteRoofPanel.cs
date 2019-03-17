using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
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
