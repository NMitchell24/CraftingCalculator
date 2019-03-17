using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class GlassCuboidRoom : ComplexRecipe
    {
        public GlassCuboidRoom()
        {
            Name = "Glass Cuboid Room";
            Type = "Base Building";
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            ChildRecipes.Add(new Glass(), 2);
        }
    }
}
