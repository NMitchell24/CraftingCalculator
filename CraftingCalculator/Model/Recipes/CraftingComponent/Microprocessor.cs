using CraftingCalculator.Model.Ingredients;


namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class Microprocessor : ComplexRecipe
    {
        public Microprocessor()
        {
            Name = "Microprocessor";
            Type = RecipeFilterLabels.CraftingComponents;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
