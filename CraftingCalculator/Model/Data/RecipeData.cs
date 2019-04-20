using System.Collections.Generic;
using LiteDB;

namespace CraftingCalculator.Model.Data
{
    public class RecipeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [BsonRef(CollectionLabels.RecipeFilters)]
        public RecipeFilterData Filter { get; set; }

        [BsonRef(CollectionLabels.IngredientQuantities)]
        public List<IngredientQuantityData> Ingredients { get; set; }
    }
}
