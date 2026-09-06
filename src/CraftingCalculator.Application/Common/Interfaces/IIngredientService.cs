using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IIngredientService
{
    Task<List<Ingredient>> GetAllIngredientsAsync();

    Task<Ingredient?> GetIngredientByIdAsync(int id);

    /// <summary>
    /// Saves or adds the ingredient. If <see cref="Ingredient.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveIngredientAsync(Ingredient? ingredient);

    Task DeleteIngredientAsync(Ingredient? ingredient);
}
