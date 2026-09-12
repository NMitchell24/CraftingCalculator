using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

// The one DAO that takes the raw IDbContextFactory instead of DatasetScopedContextFactory. It manages
// the scoping mechanism, so it has to sit outside it: routing it through the scoped factory would make
// reading the dataset list depend on a dataset already being selected, and would put a cycle between
// DatasetScopedContextFactory, ISelectedDatasetState and this DAO. Datasets carries no query filter, so
// a pooled context's leftover DatasetId has no effect on these queries.
public class DatasetDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IDatasetDAO
{
    public async Task<List<DatasetModel>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<Dataset> entities = await context.Datasets
            .AsNoTracking()
            .OrderBy(dataset => dataset.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<DatasetModel?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        Dataset? entity = await context.Datasets.AsNoTracking().FirstOrDefaultAsync(dataset => dataset.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<DatasetModel?> GetByNameAsync(string name, int exceptId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // COLLATE NOCASE rather than EF.Functions.Like: LIKE would read a user-entered % or _ in the
        // name as a wildcard, and the equality this actually wants needs no escaping. EF Core does not
        // translate the StringComparison overloads of string.Equals on SQLite, so the collation is how
        // the comparison reaches SQL. NOCASE folds ASCII only, which is what SQLite offers.
        Dataset? entity = await context.Datasets
            .AsNoTracking()
            .FirstOrDefaultAsync(dataset => dataset.Id != exceptId
                                            && EF.Functions.Collate(dataset.Name, "NOCASE") == name);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<DatasetModel> AddAsync(string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dataset entity = new() { Name = name };
        context.Datasets.Add(entity);
        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task RenameAsync(int id, string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Datasets
            .Where(dataset => dataset.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(dataset => dataset.Name, name));
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Datasets.Where(dataset => dataset.Id == id).ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        return await context.Datasets.CountAsync();
    }

    private static DatasetModel ToModel(Dataset entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name
    };
}
