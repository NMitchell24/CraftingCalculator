using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class LabLamp : Recipe
    {
        public LabLamp()
        {
            Name = "Lab Lamp";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
