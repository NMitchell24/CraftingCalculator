using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class AtmosphereHarvester : ComplexRecipe
    {
        public AtmosphereHarvester()
        {
            Name = "Atmosphere Harvester";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.AMMONIA, 100);
            ChildRecipes.Add(new MetalPlating(), 2);
            ChildRecipes.Add(new HermeticSeal(), 2);
        }
    }
}
