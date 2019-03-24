using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class UnstableGel : Recipe
    {
        public UnstableGel()
        {
            Name = "Unstable Gel";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            Ingredients.Add(IngredientType.CACTUS_FLESH, 200);
        }
    }
}
