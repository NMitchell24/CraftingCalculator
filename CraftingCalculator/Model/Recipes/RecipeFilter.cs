
namespace CraftingCalculator.Model.Recipes
{
    /// <summary>
    /// Simple Class to define a Label and Type for filtering Recipes.
    /// </summary>
    public class RecipeFilter
    {
        public string Name { get; set; }
        public RecipeType Type { get; set; }

        public RecipeFilter(string name, RecipeType type)
        {
            Name = name;
            Type = type;
        }
    }
}
