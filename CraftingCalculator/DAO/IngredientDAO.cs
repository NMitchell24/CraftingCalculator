using CraftingCalculator.Model.Data;
using System.Linq;
using LiteDB;
using System.Collections.Generic;
using CraftingCalculator.ViewModel.Ingredients;

namespace CraftingCalculator.DAO
{
    public class IngredientDAO : AbstractDAO
    {
        /// <summary>
        /// Returns a list of all IngredientData
        /// </summary>
        /// <returns></returns>
        public static List<IngredientData> GetAllIngredientData()
        {
            return _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients)
                .Find(Query.All(Query.Ascending))
                .OrderBy(x => x.Name)
                .ToList();
        }

        //Returns a list of IngredientQuantity data objects that use the provided Ingredient.
        public static List<IngredientQuantityData> GetIngredientQuantitiesForIngredient(Ingredient ingredient)
        {
            List<IngredientQuantityData> ret = new List<IngredientQuantityData>();
            var col = _data.GetCollectionByType<IngredientQuantityData>(CollectionLabels.IngredientQuantities);

            ret.AddRange(col.Include(x => x.Ingredient)
                .Find(x => x.Ingredient.Id == ingredient.Id));

            return ret;
        }

        /// <summary>
        /// Looks up an IngredientQuantity by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IngredientQuantityData GetIngredientQuantityById(int id)
        {
            var col = _data.GetCollectionByType<IngredientQuantityData>(CollectionLabels.IngredientQuantities);

            return col.Include(x => x.Ingredient).FindById(id);
        }

    }
}
