using LiteDB;
namespace CraftingCalculator.Model.Data
{
    public class IngredientQuantityData
    {
        public int Id { get; set; }

        [BsonRef(CollectionLabels.Ingredients)]
        public IngredientData Ingredient { get; set; }

        public long Quantity { get; set; }

        public IngredientQuantityData()
        {
            Id = default!;
            Ingredient = default!;
            Quantity = default!; 
        }
    }
}
