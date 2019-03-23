using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnology
{
    class OxygenRerouter : ComplexRecipe
    {
        public OxygenRerouter()
        {
            Name = "Oxygen Rerouter";
            Type = RecipeFilterLabels.ExosuitTech;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 6);
            ChildRecipes.Add(new SaltRefractor(), 1);
        }
    }
}
