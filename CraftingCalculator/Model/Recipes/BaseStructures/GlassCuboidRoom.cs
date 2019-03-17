using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
