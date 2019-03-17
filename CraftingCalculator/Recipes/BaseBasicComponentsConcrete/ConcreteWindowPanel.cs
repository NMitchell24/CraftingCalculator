using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteWindowPanel : ComplexRecipe
    {
        public ConcreteWindowPanel()
        {
            Name = "Concrete Window Panel";
            Type = "Base Building";
            Ingredients.Add(IngredientType.FERRITE_DUST, 40);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
