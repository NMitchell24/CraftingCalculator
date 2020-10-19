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

        /// <summary>
        /// Gets a record by its ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionLabel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T GetRecordById<T>(string collectionLabel, int id)
        {
            var col = _data.GetCollectionByType<T>(collectionLabel);

            return col.FindById(id);
        }

        /// <summary>
        /// Adds or updates a record.  Pass true to add, false to update.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionLabel"></param>
        /// <param name="data"></param>
        /// <param name="add"></param>
        public static void AddOrUpdateRecord<T>(string collectionLabel, object data, bool add)
        {
            var col = _data.GetCollectionByType<T>(collectionLabel);
            if (add)
            {
                col.Insert((T)data); 
            }
            else
            {
                col.Update((T)data);
            }
        }
    }
}
