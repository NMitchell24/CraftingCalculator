using LiteDB;

namespace CraftingCalculator.Model.LiteDB
{
    public class RecipeQuantityData
    {
        public int Id { get; set; }
        
        [BsonRef("Recipes")]
        public RecipeData ParentRecipe { get; set; }

        [BsonRef("Recipes")]
        public RecipeData ChildRecipe { get; set; }

        public int Quantity { get; set; }
    }
}
