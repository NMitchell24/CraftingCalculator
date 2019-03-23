using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftMiningLaser : ComplexRecipe
    {
        public ExocraftMiningLaser()
        {
            Name = "Exocraft Mining Laser";
            Type = RecipeFilterLabels.ExocraftTech;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            ChildRecipes.Add(new OxygenCapsule(), 2);
        }
    }
}
