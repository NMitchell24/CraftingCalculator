using LiteDB;

namespace CraftingCalculator.Model.Data
{
    public class RecipeQuantityData
    {
        public int Id { get; set; }
        
        [BsonRef(CollectionLabels.Recipes)]
        public RecipeData ParentRecipe { get; set; }

        [BsonRef(CollectionLabels.Recipes)]
        public RecipeData ChildRecipe { get; set; }

        public int Quantity { get; set; }
    }
}
