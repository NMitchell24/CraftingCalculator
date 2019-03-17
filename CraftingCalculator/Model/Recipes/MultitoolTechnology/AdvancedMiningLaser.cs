using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class AdvancedMiningLaser : ComplexRecipe
    {
        public AdvancedMiningLaser()
        {
            Name = "Advanced Mining Laser";
            Type = "Mining Beam Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
            ChildRecipes.Add(new HermeticSeal(), 1);
        }
    }
}
