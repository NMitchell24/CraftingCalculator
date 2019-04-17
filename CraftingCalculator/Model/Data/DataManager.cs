using LiteDB;

namespace CraftingCalculator.Model.Data
{
    public sealed class DataManager
    {
        private LiteDatabase _db;
        private DataManager()
        {
            _db = new LiteDatabase("CraftingCalculator.db");
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

        private class NestedDataManager
        {
            static NestedDataManager() { }

            internal static readonly DataManager instance = new DataManager();
        }
    }
}
