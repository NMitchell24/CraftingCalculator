using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintDAO(DatasetScopedContextFactory contextFactory) : IBlueprintDAO
{
    public async Task<BlueprintModel?> GetByIdAsync(int id)
    {
        DatasetRecords records = await LoadAsync();

        return records.Blueprints.Any(blueprint => blueprint.Id == id) ? SnapshotModelProcessor.ToBlueprintModel(records, id) : null;
    }

    public async Task<List<BlueprintModel>> GetAllAsync()
    {
        DatasetRecords records = await LoadAsync();

        return [.. SnapshotModelProcessor.ToBlueprintModels(records).OrderBy(blueprint => blueprint.Name)];
    }

    public async Task<BlueprintModel> SaveAsync(BlueprintModel blueprint)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        // The link rows are deleted and rewritten below outside the change tracker, so the transaction is what
        // keeps a failure in the second SaveChanges from leaving a blueprint with no parts.
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Blueprint entity = blueprint.Id > 0
            ? await context.Blueprints.FirstAsync(blueprintEntity => blueprintEntity.Id == blueprint.Id)
            : context.Blueprints.Add(new Blueprint()).Entity;

        entity.Name = blueprint.Name ?? "";
        entity.Description = blueprint.Description ?? "";
        entity.Value = blueprint.Value;
        entity.Yield = blueprint.Yield;
        entity.ProductionTime = blueprint.ProductionTime;
        entity.CategoryId = blueprint.Category?.Id;

        // Saved first: a new blueprint's link rows need its Id.
        await context.SaveChangesAsync();

        // Replaced wholesale rather than diffed, the way BlueprintFavoritesDAO.SaveAsync writes a favorite's
        // blueprints. Nothing holds a foreign key to a link row, so its id is free to change on every save.
        await context.BlueprintComponents.Where(link => link.BlueprintId == entity.Id).ExecuteDeleteAsync();
        await context.BlueprintChildren.Where(link => link.ParentBlueprintId == entity.Id).ExecuteDeleteAsync();

        context.BlueprintComponents.AddRange(blueprint.Components.ComponentList.Select(componentQuantity => new BlueprintComponent
        {
            BlueprintId = entity.Id,
            ComponentId = componentQuantity.Component.Id,
            Quantity = componentQuantity.Quantity
        }));

        context.BlueprintChildren.AddRange(blueprint.ChildBlueprints.BlueprintList.Select(blueprintQuantity => new BlueprintChild
        {
            ParentBlueprintId = entity.Id,
            ChildBlueprintId = blueprintQuantity.Blueprint.Id,
            Quantity = blueprintQuantity.Quantity
        }));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        blueprint.Id = entity.Id;
        return blueprint;
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        await context.Blueprints.Where(blueprintEntity => blueprintEntity.Id == id).ExecuteDeleteAsync();
    }

    private async Task<DatasetRecords> LoadAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        return await DatasetRecordsReader.ReadAsync(context);
    }
}
