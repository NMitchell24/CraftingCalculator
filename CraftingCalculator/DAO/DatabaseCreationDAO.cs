using CraftingCalculator.Model.Data;

namespace CraftingCalculator.DAO
{
    public static class DatabaseCreationDAO
    {
        private static DataManager _data = DataManager.Instance;
        /// <summary>
        /// This method will make sure the database exists, all the correct Documents (tables) exist, 
        /// and that the appropriate indexes are created
        /// </summary>
        public static void EnsureDatabaseExists()
        {
            //Gets ingredients.  creates Collection if it does not exist.
            var ing = _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients);
            //Create indexes for ingredients if they do not exist.
            ing.EnsureIndex(x => x.Name);
            ing.EnsureIndex(x => x.Id);

            //Get Filters and create indexes.
            var filt = _data.GetCollectionByType<RecipeFilterData>(CollectionLabels.RecipeFilters);
            filt.EnsureIndex(x => x.Name);
            filt.EnsureIndex(x => x.Id);

            //Ingredient Quantity objects.
            var ingQ = _data.GetCollectionByType<IngredientQuantityData>(CollectionLabels.IngredientQuantities);
            ingQ.EnsureIndex(x => x.Id);
            ingQ.EnsureIndex(x => x.Ingredient.Id);

            //recipe objects.
            var rec = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);
            rec.EnsureIndex(x => x.Id);
            rec.EnsureIndex(x => x.Filter.Id);
            rec.EnsureIndex(x => x.Name);

            //Recipe quantities
            var recQ = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            recQ.EnsureIndex(x => x.Id);
            recQ.EnsureIndex(x => x.ChildRecipe.Id);
            recQ.EnsureIndex(x => x.ParentRecipe.Id);

            //Recipe Favorites
            var favs = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
            favs.EnsureIndex(x => x.Id);
            favs.EnsureIndex(x => x.Name);

            //Favorites Recipe Quantities
            var favRecs = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);
            favRecs.EnsureIndex(x => x.Id);
            favRecs.EnsureIndex(x => x.Favorite.Id);
            favRecs.EnsureIndex(x => x.Recipe.Id);
        }
    }
}
