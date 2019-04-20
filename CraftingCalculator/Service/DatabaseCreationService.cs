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
    }
}
