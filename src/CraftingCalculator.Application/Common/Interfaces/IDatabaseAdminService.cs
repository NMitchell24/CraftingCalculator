namespace CraftingCalculator.Application.Common.Interfaces;

public interface IDatabaseAdminService
{
    /// <summary>
    /// Deletes all data from the database, leaving only the seeded <see cref="Domain.Models.RecipeFilter.ALL"/> filter.
    /// </summary>
    Task DeleteAllDataAsync();
}
