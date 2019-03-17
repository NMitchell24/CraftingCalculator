
namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class LivingGlass : ComplexRecipe
    {
        public LivingGlass()
        {
            Name = "Living Glass";
            Type = "Advanced Agricultural Product";
            ChildRecipes.Add(new Lubricant(), 1);
            ChildRecipes.Add(new Glass(), 5);
        }
    }
}
