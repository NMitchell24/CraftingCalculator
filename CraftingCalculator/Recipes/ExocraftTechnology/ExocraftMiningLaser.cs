using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.Consumable;

namespace CraftingCalculator.Recipes.ExocraftTechnology
{
    class ExocraftMiningLaser : ComplexRecipe
    {
        public ExocraftMiningLaser()
        {
            Name = "Exocraft Mining Laser";
            Type = "Exocraft Module";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            ChildRecipes.Add(new OxygenCapsule(), 2);
        }
    }
}
