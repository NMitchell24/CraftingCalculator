using CraftingCalculator.Model.Data;
using System.Linq;
using LiteDB;
using System.Collections.Generic;

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
    }
}
