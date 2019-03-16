using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class BarrelIoniser : ComplexRecipe
    {
        public BarrelIoniser()
        {
            Name = "Barrel Ioniser";
            Type = "Boltcaster Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
