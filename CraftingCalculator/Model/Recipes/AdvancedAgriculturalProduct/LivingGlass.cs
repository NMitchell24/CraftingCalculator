
namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class LivingGlass : ComplexRecipe
    {
        public LivingGlass()
        {
            Name = "Living Glass";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            ChildRecipes.Add(new Lubricant(), 1);
            ChildRecipes.Add(new Glass(), 5);
        }
    }
}
