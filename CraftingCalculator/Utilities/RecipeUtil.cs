using System.Collections.Generic;
using System.Linq;
using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.Model.Recipes;
using CraftingCalculator.Model.Ingredients;


namespace CraftingCalculator.Utilities
{
    /// <summary>
    /// For looking up lists of recipes.  
    /// Separate Functions are provided to get lists for UI Filters.
    /// New Recipes should be added into the appropriate Get function 
    /// so that they are displayed in the proper filter.
    /// If a new Get is added for a new filter, then it should also be added 
    /// into the GetAll function so that the new recipes are included in the 'All' filter.
    /// </summary>
    public static class RecipeUtil
    {
        
        /// <summary>
        /// Get a list of RecipeFilters
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFilter> GetRecipeFilters()
        {
            List<RecipeFilter> ret = new List<RecipeFilter>();

            foreach(RecipeFilterData data in CraftingCalculatorDAO.GetAllRecipeFiltersData())
            {
                RecipeFilter filter = new RecipeFilter
                {
                    Name = data.Name,
                    Id = data.Id
                };
                ret.Add(filter);
            }

            return ret;
        }

        /// <summary>
        /// Loads all Recipes for a provided RecipeFilter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<Recipe> GetRecipesByFilter(RecipeFilter filter)
        {
            List<Recipe> ret = new List<Recipe>();

            foreach (RecipeData data in CraftingCalculatorDAO.GetRecipeDataByFilter(filter))
            {
                ret.Add(GetRecipeForData(data));
            }

            return ret;
        }

        /// <summary>
        /// Returns all the Recipe Favorite objects.
        /// </summary>
        /// <returns></returns>
        public static List<RecipeFavorite> GetAllRecipeFavorites()
        {
            List<RecipeFavorite> ret = new List<RecipeFavorite>();

            foreach (RecipeFavoritesData data in CraftingCalculatorDAO.GetAllRecipeFavorites())
            {
                RecipeFavorite fav = new RecipeFavorite
                {
                    Name = data.Name,
                    Id = data.Id
                };
                ret.Add(fav);
            }
            return ret;
        }

        /// <summary>
        /// Saves or updates a Recipe Favorite
        /// </summary>
        /// <param name="fav"></param>
        public static void SaveRecipeFavorite(RecipeFavorite fav, List<RecipeQuantity> quantities)
        {
            RecipeFavoritesData data = new RecipeFavoritesData()
            {
                Name = fav.Name
            };

            if(fav.Id > 0)
            {
                data.Id = fav.Id;
            }
            //Save or update the Favorite object first.
            CraftingCalculatorDAO.SaveRecipeFavorite(data);

            //Look the favorite back up from the DB to make sure we get the correct ID to reference.
            data = CraftingCalculatorDAO.GetFavoriteByName(fav.Name);

            //Build a list for the recipe quantities.
            List<FavoriteRecipeQuantitiesData> favsData = new List<FavoriteRecipeQuantitiesData>();

            foreach(RecipeQuantity q in quantities)
            {
                RecipeData recData = CraftingCalculatorDAO.GetRecipeById(q.Recipe.Id);
                FavoriteRecipeQuantitiesData recFavData = new FavoriteRecipeQuantitiesData()
                {
                    Favorite = data,
                    Recipe = recData,
                    Quantity = q.Quantity
                };

                favsData.Add(recFavData);
            }

            //Save the recipe quantities for this favorite.
            CraftingCalculatorDAO.SaveAllFavoritesRecipeQuantites(favsData);
        }

        /// <summary>
        /// Check to see if a Favorite already exists in the DB
        /// </summary>
        /// <param name="fav"></param>
        /// <returns></returns>
        public static bool DoesFavoriteExist(string fav)
        {
            return (CraftingCalculatorDAO.GetFavoriteByName(fav) != null);
        }

        public static List<RecipeQuantity> GetRecipeQuantitiesForFavorite(RecipeFavorite fav)
        {
            List<RecipeQuantity> ret = new List<RecipeQuantity>();

            List<FavoriteRecipeQuantitiesData> data = CraftingCalculatorDAO.GetRecipeQuantitesForFavoriteId(fav.Id);

            foreach(FavoriteRecipeQuantitiesData favRecData in data)
            {
                RecipeData recData = CraftingCalculatorDAO.GetRecipeById(favRecData.Id);
                Recipe rec = GetRecipeForData(recData);
                ret.Add(new RecipeQuantity(rec, favRecData.Quantity));
            }

            return ret;
        }

        /// <summary>
        /// Builds a Recipe wrapper object from a provided RecipeData object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Recipe GetRecipeForData(RecipeData data)
        {
            Recipe ret = new Recipe
            {
                Name = data.Name,
                Description = data.Description,
                Type = data.Filter.Name,
                Id = data.Id
            };

            IngredientMap ingMap = new IngredientMap();
            foreach(IngredientQuantityData iData in data.Ingredients)
            {
                IngredientData ing = CraftingCalculatorDAO.GetIngredientById(iData.Ingredient.Id);
                ingMap.Add(ing.Name, ing.Description, iData.Quantity);
            }

            ret.Ingredients = ingMap;

            List<RecipeQuantityData> children = CraftingCalculatorDAO.GetRecipeQuantityByParentId(data.Id);

            if(children != null && children.Count > 0)
            {
                RecipeMap recMap = new RecipeMap();
                foreach(RecipeQuantityData recQ in children)
                {
                    RecipeData childData = CraftingCalculatorDAO.GetRecipeById(recQ.ChildRecipe.Id);
                    Recipe child = GetRecipeForData(childData);
                    recMap.Add(child, recQ.Quantity);
                }
                ret.ChildRecipes = recMap;
            }

            return ret;
        }

    }
}
