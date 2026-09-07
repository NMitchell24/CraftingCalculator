namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatabaseAdminDAO
{
    /// <summary>
    /// Deletes every component, blueprint, category (other than <see cref="Domain.Models.Category.ALL"/>),
    /// favorite, and their components, leaving an empty database.
    /// </summary>
    Task DeleteAllDataAsync();
}
