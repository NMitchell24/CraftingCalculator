using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class MindArc : ComplexRecipe
    {
        public MindArc()
        {
            Name = "Mind Arc";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            ChildRecipes.Add(new LivingGlass(), 1);
            ChildRecipes.Add(new Microprocessor(), 1);
        }
    }
}
