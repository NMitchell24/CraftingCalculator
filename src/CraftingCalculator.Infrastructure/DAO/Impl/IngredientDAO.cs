using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using IngredientEntity = CraftingCalculator.Domain.Entities.Ingredient;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class IngredientDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IIngredientDAO
{
    public async Task<List<Ingredient>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<IngredientEntity> entities = await context.Ingredients
            .AsNoTracking()
            .OrderBy(i => i.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        IngredientEntity? entity = await context.Ingredients.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<Ingredient> SaveAsync(Ingredient ingredient)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        IngredientEntity entity = ingredient.Id > 0
            ? await context.Ingredients.FirstAsync(i => i.Id == ingredient.Id)
            : new IngredientEntity();

        entity.Name = ingredient.Name ?? "";
        entity.Description = ingredient.Description ?? "";
        entity.Cost = ingredient.Cost;

        if (entity.Id == 0)
        {
            context.Ingredients.Add(entity);
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Ingredients.Where(i => i.Id == id).ExecuteDeleteAsync();
    }

    private static Ingredient ToModel(IngredientEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost
    };
}
