using CraftingCalculator.Model.Data;

namespace CraftingCalculator.DAO
{
    public class DatabaseCreationDAO : AbstractDAO
    {
        /// <summary>
        /// This method will make sure the database exists, all the correct Documents (tables) exist, 
        /// and that the appropriate indexes are created
        /// </summary>
        public static void EnsureDatabaseExists()
        {
            UpdateRecipeQuantitiesToLong();
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
            ingQ.EnsureIndex(x => x.Ingredient);

            //recipe objects.
            var rec = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);
            rec.EnsureIndex(x => x.Id);
            rec.EnsureIndex(x => x.Filter);
            rec.EnsureIndex(x => x.Name);
            rec.EnsureIndex(x => x.Ingredients);

            //Recipe quantities
            var recQ = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            recQ.EnsureIndex(x => x.Id);
            recQ.EnsureIndex(x => x.ChildRecipe);
            recQ.EnsureIndex(x => x.ParentRecipe);

            //Recipe Favorites
            var favs = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
            favs.EnsureIndex(x => x.Id);
            favs.EnsureIndex(x => x.Name);

            //Favorites Recipe Quantities
            var favRecs = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);
            favRecs.EnsureIndex(x => x.Id);
            favRecs.EnsureIndex(x => x.Favorite);
            favRecs.EnsureIndex(x => x.Recipe);
        }

        public static void UpdateRecipeQuantitiesToLong()
        {
            //LiteDatabase db = _data.GetDatabase();
            //if (db.UserVersion == 0)
            //{
            //    foreach (var doc in db.Engine.FindAll(CollectionLabels.RecipeQuantities)) 
            //    {
            //        doc["Quantity"] = Convert.ToInt64(doc["Quantity"].AsString);
            //        db.Engine.Update(CollectionLabels.RecipeQuantities, doc);
            //    }

            //    foreach (var doc in db.Engine.FindAll(CollectionLabels.FavoriteRecipeQuantities))
            //    {
            //        doc["Quantity"] = Convert.ToInt64(doc["Quantity"].AsString);
            //        db.Engine.Update(CollectionLabels.FavoriteRecipeQuantities, doc);
            //    }

            //    db.UserVersion = 1;
            //}
        }

        /// <summary>
        /// Deletes all records in the database from all collections.  Used for completely wiping out all data to start over with fresh empty dataset.
        /// </summary>
        public static void DeleteAllRecords()
        {
            //Delete Recipes
            var recipes = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);
            recipes.DeleteAll();
            //Delete Filters
            var recipeFilters = _data.GetCollectionByType<RecipeFilterData>(CollectionLabels.RecipeFilters);
            //Don't delete all for filters.  Make sure we leave the default 'All' filter alone.
            recipeFilters.DeleteMany(x => x.Id > 1);
            //Delete Ingredients
            var ingredients = _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients);
            ingredients.DeleteAll();
            //Delete Favorites
            var recipeFavorites = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
            recipeFavorites.DeleteAll();
            //Delete FavoriteRecipeQuantities
            var recipeFavQuantities = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);
            recipeFavQuantities.DeleteAll();
            //Delete IngredientQuantities
            var ingredientQuantities = _data.GetCollectionByType<IngredientQuantityData>(CollectionLabels.IngredientQuantities);
            ingredientQuantities.DeleteAll();
            //Delete RecipeQuantities
            var recipeQuantities = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            recipeQuantities.DeleteAll();
        }
    }
}
