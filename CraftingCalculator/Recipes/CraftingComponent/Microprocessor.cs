using CraftingCalculator.Ingredients;


namespace CraftingCalculator.Recipes.CraftingComponent
{
    class Microprocessor : ComplexRecipe
    {
        public Microprocessor()
        {
            Name = "Microprocessor";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
