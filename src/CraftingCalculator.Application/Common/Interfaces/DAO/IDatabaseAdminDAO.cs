namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatabaseAdminDAO
{
    /// <summary>
    /// Deletes every component, blueprint, category, favorite, and their components from the selected
    /// dataset, leaving it empty. Other datasets are untouched.
    /// </summary>
    Task DeleteAllDataAsync();
}
