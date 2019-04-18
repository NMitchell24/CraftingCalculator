using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Recipes;
using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.DAO
{
    public static class CraftingCalculatorDAO
    {
        public const string ALL = "All";
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

        /// <summary>
        /// Gets all the Recipe Filter data objects
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilterData> GetAllRecipeFiltersData()
        {
            var col = _data.GetCollectionByType<RecipeFilterData>(CollectionLabels.RecipeFilters);
            List<RecipeFilterData> ret = new List<RecipeFilterData>();

            ret.AddRange(col.Find(Query.All(Query.Ascending))
                .OrderByDescending(x => x.Name == ALL).ThenBy(x => x.Name));
            return ret;
        }

        /// <summary>
        /// Returns all Recipes for a given filter.   
        /// If the Filter name is 'All' then we will return all recipes.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<RecipeData> GetRecipeDataByFilter(RecipeFilter filter)
        {
            List<RecipeData> ret = new List<RecipeData>();
            var col = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);

            if (filter.Name == ALL)
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(Query.All(Query.Ascending))
                .OrderBy(x => x.Name));
            }
            else
            {
                ret.AddRange(col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .Find(x => x.Filter.Id == filter.Id)
                .OrderBy(x => x.Name));
            }
            
            return ret;
        }

        /// <summary>
        /// Gets an Ingredient by its Identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IngredientData GetIngredientById(int id)
        {
            var col = _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients);

            return col.FindById(id);
        }
        
        /// <summary>
        /// Gets all Recipe Quantity objects for a given Parent Recipe id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<RecipeQuantityData> GetRecipeQuantityByParentId(int id)
        {
            List<RecipeQuantityData> ret = new List<RecipeQuantityData>();
            var col = _data.GetCollectionByType<RecipeQuantityData>(CollectionLabels.RecipeQuantities);

            ret.AddRange(col.Include(x => x.ParentRecipe)
                .Include(x => x.ChildRecipe)
                .Find(x => x.ParentRecipe.Id == id));

            return ret;
        }

        /// <summary>
        /// Gets a Recipe by the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static RecipeData GetRecipeById(int id)
        {
            var col = _data.GetCollectionByType<RecipeData>(CollectionLabels.Recipes);

            return col.Include(x => x.Ingredients)
                .Include(x => x.Filter)
                .FindById(id);
        }

        /// <summary>
        /// Returns all the Recipe Favorites
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFavoritesData> GetAllRecipeFavorites()
        {
            List<RecipeFavoritesData> ret = new List<RecipeFavoritesData>();
            var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);

            ret.AddRange(col.Find(Query.All(Query.Ascending))
                .OrderBy(x => x.Name));

            return ret;
                
        }

        public static List<FavoriteRecipeQuantitiesData> GetRecipeQuantitesForFavoriteId(int id)
        {
            List<FavoriteRecipeQuantitiesData> ret = new List<FavoriteRecipeQuantitiesData>();
            var col = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);

            ret.AddRange(col.Include(x => x.Favorite)
                .Include(x => x.Recipe)
                .Find(x => x.Favorite.Id == id));

            return ret;

        }

        /// <summary>
        /// Saves or updates a RecipeFavoritesData object to the database.
        /// </summary>
        /// <param name="fav"></param>
        public static void SaveRecipeFavorite(RecipeFavoritesData fav)
        {
            var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);

            col.Upsert(fav);            
        }

        /// <summary>
        /// Looks up a favorite by name.
        /// Using Name instead of ID to make sure that Names stay unique.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static RecipeFavoritesData GetFavoriteByName(string name)
        {
            var col = _data.GetCollectionByType<RecipeFavoritesData>(CollectionLabels.RecipeFavorites);
            return col.FindOne(x => x.Name == name);
        }

        /// <summary>
        /// Saves or updates a FavoriteRecipeQuantitiesData object to the database.
        /// </summary>
        /// <param name="favs"></param>
        public static void SaveAllFavoritesRecipeQuantites(List<FavoriteRecipeQuantitiesData> favs)
        {
            var col = _data.GetCollectionByType<FavoriteRecipeQuantitiesData>(CollectionLabels.FavoriteRecipeQuantities);

            foreach(FavoriteRecipeQuantitiesData data in favs)
            {
                col.Upsert(data);
            }
        }
        
    }
}
