using LiteDB;
using System.IO;

namespace CraftingCalculator.DAO
{
    public sealed class DataManager
    {
        private LiteDatabase _db;
        private DataManager()
        {
            // Creates Default Database from No Mans Sky configuration if no Database currently exists.
            // Prevents program from starting with blank DB.
            if (!File.Exists(Properties.Resources.dbName))
            {
                File.WriteAllBytes(Properties.Resources.dbName, Properties.Resources.CraftingCalculator);
            }

            _db = new LiteDatabase(Properties.Resources.dbName);
        }

        public static DataManager Instance
        {
            get
            {
                return NestedDataManager.instance;
            }
        }

        public LiteDatabase GetDatabase()
        {
            return _db;
        }

        public LiteCollection<T> GetCollectionByType<T>(string label)
        {
            return _db.GetCollection<T>(label);
        }

        private class NestedDataManager
        {
            static NestedDataManager() { }

            internal static readonly DataManager instance = new DataManager();
        }
    }
}
