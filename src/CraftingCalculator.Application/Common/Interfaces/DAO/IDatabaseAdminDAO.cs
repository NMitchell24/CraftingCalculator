using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatabaseAdminDAO
{
    /// <summary>
    /// Deletes every component, blueprint, category (other than <see cref="CategoryModel.ALL"/>),
    /// favorite, and their components, leaving an empty database.
    /// </summary>
    Task DeleteAllDataAsync();
}
