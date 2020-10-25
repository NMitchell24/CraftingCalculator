using CraftingCalculator.Model.Data;
using LiteDB;
using System;

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
            ingQ.EnsureIndex(x => x.Ingredient.Id);

            //recipe objects.
            var rec = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);
            rec.EnsureIndex(x => x.Id);
            rec.EnsureIndex(x => x.Filter.Id);
            rec.EnsureIndex(x => x.Name);
            rec.EnsureIndex(x => x.Ingredients[0].Id);

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

        public static void UpdateRecipeQuantitiesToLong()
        {
            LiteDatabase db = _data.GetDatabase();
            if (db.Engine.UserVersion == 0)
            {
                foreach (var doc in db.Engine.FindAll(CollectionLabels.RecipeQuantities)) 
                {
                    doc["Quantity"] = Convert.ToInt64(doc["Quantity"].AsString);
                    db.Engine.Update(CollectionLabels.RecipeQuantities, doc);
                }

                db.Engine.UserVersion = 1;
            }
        }

        /// <summary>
        /// Deletes all records in the database from all collections.  Used for completely wiping out all data to start over with fresh empty dataset.
        /// </summary>
        public static void DeleteAllRecords()
        {
            //Delete Recipes
            var recipes = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);
            recipes.Delete(Query.All());
            //Delete Filters
            var recipeFilters = _data.GetCollectionByType<RecipeFilterData>(CollectionLabels.RecipeFilters);
            //Don't delete all for filters.  Make sure we leave the default 'All' filter alone.
            recipeFilters.Delete(x => x.Id > 1);
            //Delete Ingredients
            var ingredients = _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients);
            ingredients.Delete(Query.All());
            //Delete Favorites
            var recipeFavorites = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
            recipeFavorites.Delete(Query.All());
            //Delete FavoriteRecipeQuantities
            var recipeFavQuantities = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);
            recipeFavQuantities.Delete(Query.All());
            //Delete IngredientQuantities
            var ingredientQuantities = _data.GetCollectionByType<IngredientQuantityData>(CollectionLabels.IngredientQuantities);
            ingredientQuantities.Delete(Query.All());
            //Delete RecipeQuantities
            var recipeQuantities = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);
            recipeQuantities.Delete(Query.All());
        }
    }
}
