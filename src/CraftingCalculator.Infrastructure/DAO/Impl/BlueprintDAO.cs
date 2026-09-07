using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using ComponentEntity = CraftingCalculator.Domain.Entities.Component;
using BlueprintChildEntity = CraftingCalculator.Domain.Entities.BlueprintChild;
using BlueprintEntity = CraftingCalculator.Domain.Entities.Blueprint;
using BlueprintFilterEntity = CraftingCalculator.Domain.Entities.BlueprintFilter;
using BlueprintComponentEntity = CraftingCalculator.Domain.Entities.BlueprintComponent;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IBlueprintDAO
{
    public async Task<List<Blueprint>> GetByFilterAsync(BlueprintFilter filter)
    {
        BlueprintGraph graph = await LoadGraphAsync();

        IEnumerable<BlueprintEntity> matching = filter.Name == BlueprintFilter.ALL
            ? graph.BlueprintsById.Values
            : graph.BlueprintsById.Values.Where(r => r.FilterId == filter.Id);

        return [.. matching.OrderBy(r => r.Name).Select(r => BuildModel(r, graph, 0))];
    }

    public async Task<Blueprint?> GetByIdAsync(int id)
    {
        BlueprintGraph graph = await LoadGraphAsync();

        return graph.BlueprintsById.TryGetValue(id, out BlueprintEntity? entity) ? BuildModel(entity, graph, 0) : null;
    }

    public async Task<List<Blueprint>> GetAllAsync()
    {
        BlueprintGraph graph = await LoadGraphAsync();

        return [.. graph.BlueprintsById.Values.OrderBy(r => r.Name).Select(r => BuildModel(r, graph, 0))];
    }

    public async Task<Blueprint> SaveAsync(Blueprint blueprint)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        BlueprintEntity entity = blueprint.Id > 0
            ? await context.Blueprints.Include(r => r.Components).FirstAsync(r => r.Id == blueprint.Id)
            : new BlueprintEntity();

        entity.Name = blueprint.Name ?? "";
        entity.Description = blueprint.Description ?? "";
        entity.Value = blueprint.Value;
        entity.FilterId = blueprint.Filter?.Id;

        if (entity.Id == 0)
        {
            context.Blueprints.Add(entity);
        }

        foreach (ComponentQuantity removed in blueprint.Components.RemovedComponents.Where(i => i.Id > 0))
        {
            BlueprintComponentEntity? toRemove = entity.Components.FirstOrDefault(ri => ri.Id == removed.Id);
            if (toRemove != null)
            {
                context.BlueprintComponents.Remove(toRemove);
                entity.Components.Remove(toRemove);
            }
        }

        foreach (ComponentQuantity iq in blueprint.Components.ComponentList)
        {
            if (iq.Id > 0)
            {
                entity.Components.First(ri => ri.Id == iq.Id).Quantity = iq.Quantity;
            }
            else
            {
                entity.Components.Add(new BlueprintComponentEntity { ComponentId = iq.Component.Id, Quantity = iq.Quantity });
            }
        }

        // Save the parent blueprint now: a newly-added child BlueprintChild row below needs the parent's
        // (possibly newly-assigned) Id.
        await context.SaveChangesAsync();

        foreach (BlueprintQuantity removed in blueprint.ChildBlueprints.RemovedBlueprints.Where(r => r.Id > 0))
        {
            await context.BlueprintChildren.Where(rc => rc.Id == removed.Id).ExecuteDeleteAsync();
        }

        foreach (BlueprintQuantity rq in blueprint.ChildBlueprints.BlueprintList)
        {
            if (rq.Id > 0)
            {
                BlueprintChildEntity existing = await context.BlueprintChildren.FirstAsync(rc => rc.Id == rq.Id);
                existing.Quantity = rq.Quantity;
            }
            else
            {
                context.BlueprintChildren.Add(new BlueprintChildEntity
                {
                    ParentBlueprintId = entity.Id,
                    ChildBlueprintId = rq.Blueprint.Id,
                    Quantity = rq.Quantity
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
        await context.Blueprints.Where(r => r.Id == id).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Loads every blueprint, component, filter, and component link in five queries so the component
    /// graph can be stitched together in memory instead of one query per node (the shape the old
    /// LiteDB-backed recursive loader used, which the plan calls out as the wrong fit for EF Core).
    /// </summary>
    private async Task<BlueprintGraph> LoadGraphAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dictionary<int, ComponentEntity> componentsById =
            await context.Components.AsNoTracking().ToDictionaryAsync(i => i.Id);
        Dictionary<int, BlueprintFilterEntity> filtersById =
            await context.BlueprintFilters.AsNoTracking().ToDictionaryAsync(f => f.Id);
        Dictionary<int, BlueprintEntity> blueprintsById =
            await context.Blueprints.AsNoTracking().ToDictionaryAsync(r => r.Id);
        ILookup<int, BlueprintComponentEntity> componentsByBlueprintId =
            (await context.BlueprintComponents.AsNoTracking().ToListAsync()).ToLookup(ri => ri.BlueprintId);
        ILookup<int, BlueprintChildEntity> childrenByParentId =
            (await context.BlueprintChildren.AsNoTracking().ToListAsync()).ToLookup(rc => rc.ParentBlueprintId);

        return new BlueprintGraph(blueprintsById, componentsById, filtersById, componentsByBlueprintId, childrenByParentId);
    }

    /// <summary>
    /// Recursively hydrates a full <see cref="Blueprint"/> model (components and, recursively, child
    /// blueprints) from the in-memory graph. The app does not otherwise detect a cycle in the blueprint
    /// graph (see the "no cycle guard" parity issue), so an A -> B -> A pair would recurse
    /// indefinitely; the shared <see cref="BlueprintProcessor.MaxBlueprintDepth"/> bound turns that into a
    /// catchable exception instead of a StackOverflowException.
    /// </summary>
    private static Blueprint BuildModel(BlueprintEntity entity, BlueprintGraph graph, int depth)
    {
        if (depth > BlueprintProcessor.MaxBlueprintDepth)
        {
            throw new InvalidOperationException(
                $"Blueprint graph exceeded the maximum depth of {BlueprintProcessor.MaxBlueprintDepth}; check for a cycle involving '{entity.Name}'.");
        }

        Blueprint model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Value = entity.Value
        };

        if (entity.FilterId is int filterId && graph.FiltersById.TryGetValue(filterId, out BlueprintFilterEntity? filterEntity))
        {
            model.Filter = ToFilterModel(filterEntity);
        }

        foreach (BlueprintComponentEntity ri in graph.ComponentsByBlueprintId[entity.Id])
        {
            if (graph.ComponentsById.TryGetValue(ri.ComponentId, out ComponentEntity? componentEntity))
            {
                model.Components.Add(ToComponentModel(componentEntity), ri.Quantity, ri.Id);
            }
        }

        foreach (BlueprintChildEntity rc in graph.ChildrenByParentId[entity.Id])
        {
            if (graph.BlueprintsById.TryGetValue(rc.ChildBlueprintId, out BlueprintEntity? childEntity))
            {
                model.ChildBlueprints.Add(BuildModel(childEntity, graph, depth + 1), rc.Quantity, rc.Id);
            }
        }

        return model;
    }

    private static BlueprintFilter ToFilterModel(BlueprintFilterEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };

    private static Component ToComponentModel(ComponentEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost
    };

    private sealed record BlueprintGraph(
        Dictionary<int, BlueprintEntity> BlueprintsById,
        Dictionary<int, ComponentEntity> ComponentsById,
        Dictionary<int, BlueprintFilterEntity> FiltersById,
        ILookup<int, BlueprintComponentEntity> ComponentsByBlueprintId,
        ILookup<int, BlueprintChildEntity> ChildrenByParentId);
}
