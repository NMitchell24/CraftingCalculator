using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsWood
{
    class WoodenRoof : Recipe
    {
        public WoodenRoof()
        {
            Name = "Wooden Roof";
            Type = RecipeFilterLabels.BaseComponentsWood;
            Ingredients.Add(IngredientType.CARBON, 20);
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
        }
    }
}
