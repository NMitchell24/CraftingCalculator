using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class IngredientService(IIngredientDAO dao) : IIngredientService
{
    public Task<List<Ingredient>> GetAllIngredientsAsync() => dao.GetAllAsync();

    public Task<Ingredient?> GetIngredientByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveIngredientAsync(Ingredient? ingredient)
        => ingredient != null ? dao.SaveAsync(ingredient) : Task.CompletedTask;

    public Task DeleteIngredientAsync(Ingredient? ingredient)
        => ingredient != null ? dao.DeleteAsync(ingredient.Id) : Task.CompletedTask;
}
