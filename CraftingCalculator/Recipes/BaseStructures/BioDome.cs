using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class BioDome : ComplexRecipe
    {
        public BioDome()
        {
            Name = "Bio-Dome";
            Type = "Base Building";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
            ChildRecipes.Add(new Glass(), 5);
        }
    }
}
