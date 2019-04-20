namespace CraftingCalculator.DAO
{
    public abstract class AbstractDAO
    {
        protected static DataManager _data = DataManager.Instance;

        /// <summary>
        /// Deletes a record by its ID.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionLabel"></param>
        /// <param name="id"></param>
        public static void DeleteRecordById<T>(string collectionLabel, int id)
        {
            var col = _data.GetCollectionByType<T>(collectionLabel);
            col.Delete(id);
        }
    }
}
