namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatabaseAdminDAO
{
    /// <summary>
    /// Deletes every component, blueprint, filter (other than <see cref="Domain.Models.BlueprintFilter.ALL"/>),
    /// favorite, and their components, leaving an empty database.
    /// </summary>
    Task DeleteAllDataAsync();
}
