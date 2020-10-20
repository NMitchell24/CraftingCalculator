using CraftingCalculator.DAO;
using CraftingCalculator.Model.Data;
using CraftingCalculator.ViewModel.Ingredients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingCalculator.Service
{
    public static class IngredientService
    {
        /// <summary>
        /// Returns a list of all Ingredients
        /// </summary>
        /// <returns></returns>
        public static List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> ret = new List<Ingredient>();
            List<IngredientData> dataList = IngredientDAO.GetAllIngredientData();

            foreach(IngredientData data in dataList)
            {
                Ingredient i = GetIngredientForData(data);
                ret.Add(i);
            }

            return ret;
        }

        /// <summary>
        /// Saves or adds the Ingredient.  If the ID value is 0 a new one will be added, otherwise the existing record will be updated.
        /// </summary>
        /// <param name="ing"></param>
        public static void SaveIngredient(Ingredient ing)
        {
            IngredientData data;
            if (ing.Id > 0)
            {
                data = AbstractDAO.GetRecordById<IngredientData>(CollectionLabels.Ingredients, ing.Id);
            }
            else
            {
                data = new IngredientData();
            }

            CopyToData(ing, data);

            AbstractDAO.AddOrUpdateRecord<IngredientData>(CollectionLabels.Ingredients, data, ing.Id <= 0);          
        }

        /// <summary>
        /// Deletes the provided Ingredient.
        /// 
        /// TODO: Refactor this to be more performant.
        /// </summary>
        /// <param name="ing"></param>
        public static void DeleteIngredient(Ingredient ing)
        {
            //First look up any IngredientQuantityData that uses this Ingredient.
            List<IngredientQuantityData> recipeIngredients = IngredientDAO.GetIngredientQuantitiesForIngredient(ing);

            foreach(IngredientQuantityData ingredientQuantity in recipeIngredients.ToList())
            {
                //Lookup the recipes associated with this data.
                List<RecipeData> recipes = RecipeDAO.GetRecipeByIngredient(ingredientQuantity);

                //Loop through recipes and remove associations.
                foreach(RecipeData recipe in recipes)
                {
                    foreach(IngredientQuantityData recipeIngredient in recipe.Ingredients.ToList())
                    {
                        if(recipeIngredient.Id == ingredientQuantity.Id)
                        {
                            recipe.Ingredients.Remove(recipeIngredient);
                        }
                    }
                    AbstractDAO.AddOrUpdateRecord<RecipeData>(CollectionLabels.Recipes, recipe, false);
                }

                //Delete the Ingredient Quantity
                AbstractDAO.DeleteRecordById<IngredientQuantityData>(CollectionLabels.IngredientQuantities, ingredientQuantity.Id);
            }

            //Now delete ingredient.
            AbstractDAO.DeleteRecordById<IngredientData>(CollectionLabels.Ingredients, ing.Id);
        }

        private static void CopyToData(Ingredient ing, IngredientData data)
        {
            data.Name = ing.Name;
            data.Description = ing.Description;
            data.Cost = ing.Cost;
        }

        private static Ingredient GetIngredientForData(IngredientData data)
        {
            return new Ingredient
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                Cost = data.Cost
            };

        }
    }
}
