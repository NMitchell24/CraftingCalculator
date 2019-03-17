using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
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
