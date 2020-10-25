using CraftingCalculator.DAO;

namespace CraftingCalculator.Service
{
    public static class DatabaseCreationService
    {
        /// <summary>
        /// Creates the Database if it does not exist.  
        /// </summary>
        public static void CreateDatabase()
        {
            DatabaseCreationDAO.EnsureDatabaseExists();
        }

        /// <summary>
        /// Deletes all data from the database leaving only empty collections.
        /// </summary>
        public static void DeleteAllData()
        {
            DatabaseCreationDAO.DeleteAllRecords();
        }
    }
}
