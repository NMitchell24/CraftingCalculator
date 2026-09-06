using LiteDB;

namespace CraftingCalculator.Domain.Entities
{
    public class RecipeQuantityData
    {
        public int Id { get; set; }
        
        [BsonRef(CollectionLabels.Recipes)]
        public RecipeData ParentRecipe { get; set; }

        [BsonRef(CollectionLabels.Recipes)]
        public RecipeData ChildRecipe { get; set; }

        public long Quantity { get; set; }

        public RecipeQuantityData()
        {
            Id = default!;
            ParentRecipe = default!;
            ChildRecipe = default!;
            Quantity = default!;
        }
    }
}
