using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class PolyFibre : Recipe
    {
        public PolyFibre()
        {
            Name = "Poly Fibre";
            Type = "Advanced Agricultural Product";
            Ingredients.Add(IngredientType.CACTUS_FLESH, 100);
            Ingredients.Add(IngredientType.STAR_BULB, 200);
        }
    }
}
