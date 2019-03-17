using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class ViewingSphere : ComplexRecipe
    {
        public ViewingSphere()
        {
            Name = "Viewing Sphere";
            Type = "Base Building";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 10);
            ChildRecipes.Add(new Glass(), 3);
        }
    }
}
