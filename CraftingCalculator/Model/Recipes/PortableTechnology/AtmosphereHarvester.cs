using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class AtmosphereHarvester : ComplexRecipe
    {
        public AtmosphereHarvester()
        {
            Name = "Atmosphere Harvester";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.AMMONIA, 100);
            ChildRecipes.Add(new MetalPlating(), 2);
            ChildRecipes.Add(new HermeticSeal(), 2);
        }
    }
}
