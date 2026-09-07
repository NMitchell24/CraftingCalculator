namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatabaseAdminDAO
{
    /// <summary>
    /// Deletes every ingredient, recipe, filter (other than <see cref="Domain.Models.RecipeFilter.ALL"/>),
    /// favorite, and their components, leaving an empty database.
    /// </summary>
    Task DeleteAllDataAsync();
}
