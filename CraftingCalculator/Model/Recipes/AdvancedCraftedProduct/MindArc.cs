using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.AdvancedCraftedProduct
{
    class MindArc : ComplexRecipe
    {
        public MindArc()
        {
            Name = "Mind Arc";
            Type = "Advanced Crafted Product";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            ChildRecipes.Add(new LivingGlass(), 1);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
