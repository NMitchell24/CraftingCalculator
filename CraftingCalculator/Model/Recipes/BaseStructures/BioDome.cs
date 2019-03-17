using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.BaseStructures
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
