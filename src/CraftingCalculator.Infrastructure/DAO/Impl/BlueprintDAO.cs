using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IBlueprintDAO
{
    public async Task<List<BlueprintModel>> GetByCategoryAsync(CategoryModel category)
    {
        BlueprintGraph graph = await LoadGraphAsync();

        IEnumerable<Blueprint> matching = category.Name == CategoryModel.All
            ? graph.BlueprintsById.Values
            : graph.BlueprintsById.Values.Where(blueprintEntity => blueprintEntity.CategoryId == category.Id);

        return [.. matching.OrderBy(blueprintEntity => blueprintEntity.Name).Select(blueprintEntity => BuildModel(blueprintEntity, graph, 0))];
    }

    public async Task<BlueprintModel?> GetByIdAsync(int id)
    {
        BlueprintGraph graph = await LoadGraphAsync();

        return graph.BlueprintsById.TryGetValue(id, out Blueprint? entity) ? BuildModel(entity, graph, 0) : null;
    }

    public async Task<List<BlueprintModel>> GetAllAsync()
    {
        BlueprintGraph graph = await LoadGraphAsync();

        return [.. graph.BlueprintsById.Values.OrderBy(blueprintEntity => blueprintEntity.Name).Select(blueprintEntity => BuildModel(blueprintEntity, graph, 0))];
    }

    public async Task<BlueprintModel> SaveAsync(BlueprintModel blueprint)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Blueprint entity = blueprint.Id > 0
            ? await context.Blueprints.Include(blueprintEntity => blueprintEntity.Components).FirstAsync(blueprintEntity => blueprintEntity.Id == blueprint.Id)
            : new Blueprint();

        entity.Name = blueprint.Name ?? "";
        entity.Description = blueprint.Description ?? "";
        entity.Value = blueprint.Value;
        entity.Yield = blueprint.Yield;
        entity.ProductionTime = blueprint.ProductionTime;
        entity.CategoryId = blueprint.Category?.Id;

        if (entity.Id == 0)
        {
            context.Blueprints.Add(entity);
        }

        foreach (ComponentQuantity removed in blueprint.Components.RemovedComponents.Where(componentQuantity => componentQuantity.Id > 0))
        {
            BlueprintComponent? toRemove = entity.Components.FirstOrDefault(blueprintComponent => blueprintComponent.Id == removed.Id);
            if (toRemove != null)
            {
                context.BlueprintComponents.Remove(toRemove);
                entity.Components.Remove(toRemove);
            }
        }

        foreach (ComponentQuantity componentQuantity in blueprint.Components.ComponentList)
        {
            if (componentQuantity.Id > 0)
            {
                entity.Components.First(link => link.Id == componentQuantity.Id).Quantity = componentQuantity.Quantity;
            }
            else
            {
                entity.Components.Add(new BlueprintComponent { ComponentId = componentQuantity.Component.Id, Quantity = componentQuantity.Quantity });
            }
        }

        // Save the parent blueprint now: a newly-added child BlueprintChild row below needs the parent's
        // (possibly newly-assigned) Id.
        await context.SaveChangesAsync();

        foreach (BlueprintQuantity removed in blueprint.ChildBlueprints.RemovedBlueprints.Where(removedQuantity => removedQuantity.Id > 0))
        {
            await context.BlueprintChildren.Where(link => link.Id == removed.Id).ExecuteDeleteAsync();
        }

        foreach (BlueprintQuantity blueprintQuantity in blueprint.ChildBlueprints.BlueprintList)
        {
            if (blueprintQuantity.Id > 0)
            {
                BlueprintChild existing = await context.BlueprintChildren.FirstAsync(link => link.Id == blueprintQuantity.Id);
                existing.Quantity = blueprintQuantity.Quantity;
            }
            else
            {
                context.BlueprintChildren.Add(new BlueprintChild
                {
                    ParentBlueprintId = entity.Id,
                    ChildBlueprintId = blueprintQuantity.Blueprint.Id,
                    Quantity = blueprintQuantity.Quantity
                });
            }
        }

        await context.SaveChangesAsync();

        blueprint.Id = entity.Id;
        return blueprint;
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Blueprints.Where(blueprintEntity => blueprintEntity.Id == id).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Loads every blueprint, component, category, and component link in five queries so the component
    /// graph can be stitched together in memory instead of one query per node (the shape the old
    /// LiteDB-backed recursive loader used, which the plan calls out as the wrong fit for EF Core).
    /// </summary>
    private async Task<BlueprintGraph> LoadGraphAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dictionary<int, Component> componentsById =
            await context.Components.AsNoTracking().ToDictionaryAsync(component => component.Id);
        Dictionary<int, Category> categoriesById =
            await context.Categories.AsNoTracking().ToDictionaryAsync(category => category.Id);
        Dictionary<int, Blueprint> blueprintsById =
            await context.Blueprints.AsNoTracking().ToDictionaryAsync(blueprintEntity => blueprintEntity.Id);
        ILookup<int, BlueprintComponent> componentsByBlueprintId =
            (await context.BlueprintComponents.AsNoTracking().ToListAsync()).ToLookup(blueprintComponent => blueprintComponent.BlueprintId);
        ILookup<int, BlueprintChild> childrenByParentId =
            (await context.BlueprintChildren.AsNoTracking().ToListAsync()).ToLookup(blueprintChild => blueprintChild.ParentBlueprintId);

        return new BlueprintGraph(blueprintsById, componentsById, categoriesById, componentsByBlueprintId, childrenByParentId);
    }

    /// <summary>
    /// Recursively hydrates a full <see cref="BlueprintModel"/> model (components and, recursively, child
    /// blueprints) from the in-memory graph. The app does not otherwise detect a cycle in the blueprint
    /// graph (see the "no cycle guard" parity issue), so an A -> B -> A pair would recurse
    /// indefinitely; the shared <see cref="BlueprintProcessor.MaxBlueprintDepth"/> bound turns that into a
    /// catchable exception instead of a StackOverflowException.
    /// </summary>
    private static BlueprintModel BuildModel(Blueprint entity, BlueprintGraph graph, int depth)
    {
        if (depth > BlueprintProcessor.MaxBlueprintDepth)
        {
            throw new InvalidOperationException(
                $"Blueprint graph exceeded the maximum depth of {BlueprintProcessor.MaxBlueprintDepth}; check for a cycle involving '{entity.Name}'.");
        }

        BlueprintModel model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Value = entity.Value,
            Yield = entity.Yield,
            ProductionTime = entity.ProductionTime
        };

        if (entity.CategoryId is int categoryId && graph.CategoriesById.TryGetValue(categoryId, out Category? categoryEntity))
        {
            model.Category = ToCategoryModel(categoryEntity);
        }

        foreach (BlueprintComponent blueprintComponent in graph.ComponentsByBlueprintId[entity.Id])
        {
            if (graph.ComponentsById.TryGetValue(blueprintComponent.ComponentId, out Component? componentEntity))
            {
                model.Components.Add(ToComponentModel(componentEntity), blueprintComponent.Quantity, blueprintComponent.Id);
            }
        }

        foreach (BlueprintChild blueprintChild in graph.ChildrenByParentId[entity.Id])
        {
            if (graph.BlueprintsById.TryGetValue(blueprintChild.ChildBlueprintId, out Blueprint? childEntity))
            {
                model.ChildBlueprints.Add(BuildModel(childEntity, graph, depth + 1), blueprintChild.Quantity, blueprintChild.Id);
            }
        }

        return model;
    }

    private static CategoryModel ToCategoryModel(Category entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };

    private static ComponentModel ToComponentModel(Component entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost,
        ProductionTime = entity.ProductionTime
    };

    private sealed record BlueprintGraph(
        Dictionary<int, Blueprint> BlueprintsById,
        Dictionary<int, Component> ComponentsById,
        Dictionary<int, Category> CategoriesById,
        ILookup<int, BlueprintComponent> ComponentsByBlueprintId,
        ILookup<int, BlueprintChild> ChildrenByParentId);
}
