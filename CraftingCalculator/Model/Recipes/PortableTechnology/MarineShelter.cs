using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class MarineShelter : ComplexRecipe
    {
        public MarineShelter()
        {
            Name = "Marine Shelter";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.LIVING_PEARL, 5);
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
