using LiteDB;

namespace CraftingCalculator.Model.Data
{
    public class FavoriteRecipeQuantitiesData
    {
        public int Id { get; set; }

        [BsonRef(CollectionLabels.RecipeFavorites)]
        public RecipeFavoritesData Favorite { get; set; }
        
        [BsonRef(CollectionLabels.Recipes)]
        public RecipeData Recipe { get; set; }

        public long Quantity { get; set; }

        public FavoriteRecipeQuantitiesData()
        {
            Id = default!;
            Recipe = default!;
            Quantity = default!;
            Favorite = default!;
        }
    }
}
