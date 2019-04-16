using LiteDB;
namespace CraftingCalculator.Model.LiteDB
{
    public class IngredientQuantityData
    {
        public int Id { get; set; }

        [BsonRef("Ingredients")]
        public IngredientData Ingredient { get; set; }

        public long Quantity { get; set; }
    }
}
