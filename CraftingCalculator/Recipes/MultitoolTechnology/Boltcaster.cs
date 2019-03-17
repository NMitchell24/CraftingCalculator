using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.MultitoolTechnology
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
