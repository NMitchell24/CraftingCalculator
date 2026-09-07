using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IIngredientDAO
{
    Task<List<Ingredient>> GetAllAsync();

    Task<Ingredient?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the ingredient if <see cref="Ingredient.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved ingredient with its assigned <see cref="Ingredient.Id"/>.
    /// </summary>
    Task<Ingredient> SaveAsync(Ingredient ingredient);

    Task DeleteAsync(int id);
}
