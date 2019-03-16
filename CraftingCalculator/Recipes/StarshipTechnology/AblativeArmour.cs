using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class AblativeArmour : ComplexRecipe
    {
        public AblativeArmour()
        {
            Name = "Ablative Armour";
            Type = "Starship Shield Module";
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 50);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
