using CraftingCalculator.Ingredients;


namespace CraftingCalculator.Recipes
{
    class Microprocessor : ComplexRecipe
    {
        public Microprocessor()
        {
            Name = "Microprocessor";
            Type = "Component";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
