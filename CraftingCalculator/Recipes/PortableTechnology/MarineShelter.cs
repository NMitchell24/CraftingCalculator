using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class MarineShelter : ComplexRecipe
    {
        public MarineShelter()
        {
            Name = "Marine Shelter";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.LIVING_PEARL, 5);
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            ChildRecipes.Add(new Glass(), 1);
        }
    }
}
