using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class SmallLanternLight : Recipe
    {
        public SmallLanternLight()
        {
            Name = "Light (Small Lamp Style)";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 5);
        }
    }
}
