using CraftingCalculator.Model.Data;
using System.Linq;
using LiteDB;
using System.Collections.Generic;

namespace CraftingCalculator.DAO
{
    public class IngredientDAO : AbstractDAO
    {
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

        /// <summary>
        /// If the data record has an id value greater than 0 then this method will update the record.  If ID value is 0 then we will add a new record.
        /// </summary>
        /// <param name="data"></param>
        public static void AddOrUpdateRecord(IngredientData data)
        {
            var col = _data.GetCollectionByType<IngredientData>(CollectionLabels.Ingredients);
            if (data.Id > 0)
            {
                col.Update(data);
            }
            else
            {
                col.Insert(data);
            }
        }
    }
}
