using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class WeaponRack : Recipe
    {
        public WeaponRack()
        {
            Name = "Weapon Rack";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
