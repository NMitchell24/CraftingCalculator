using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class JetpackRepair : Recipe
    {
        public JetpackRepair()
        {
            Name = "Jetpack (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.FERRITE_DUST, 50);
        }
    }
}
