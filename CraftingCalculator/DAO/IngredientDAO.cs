using CraftingCalculator.Model.Data;

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
    }
}
