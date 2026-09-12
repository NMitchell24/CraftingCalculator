using CraftingCalculator.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure;

/// <summary>
/// Hands out contexts already scoped to the selected dataset. DAOs take this rather than
/// <see cref="IDbContextFactory{TContext}"/> so an unscoped context is not something a DAO can end up
/// holding by omission.
/// </summary>
public sealed class DatasetScopedContextFactory(
    IDbContextFactory<CraftingDataContext> contextFactory,
    ISelectedDatasetState selectedDataset)
{
    public async Task<CraftingDataContext> CreateAsync()
    {
        // A guard rather than a fallback: id 0 matches no row, so every screen would render empty and
        // every save would land in a dataset that does not exist. Failing here names the cause.
        if (selectedDataset.Id == 0)
        {
            throw new InvalidOperationException(
                "No dataset is selected. IDatasetService.InitializeAsync must run before any data is read.");
        }

        CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        context.DatasetId = selectedDataset.Id;

        return context;
    }
}
