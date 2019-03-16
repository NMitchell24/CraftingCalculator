using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.AdvancedCraftedProduct
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
