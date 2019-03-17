using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class Boltcaster : ComplexRecipe
    {
        public Boltcaster()
        {
            Name = "Boltcaster";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
            ChildRecipes.Add(new CarbonNanotubes(), 3);
        }
    }
}
