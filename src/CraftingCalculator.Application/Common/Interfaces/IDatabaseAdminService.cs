using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IDatabaseAdminService
{
    /// <summary>
    /// Deletes all data from the database, leaving only the seeded <see cref="CategoryModel.All"/> category.
    /// </summary>
    Task DeleteAllDataAsync();
}
