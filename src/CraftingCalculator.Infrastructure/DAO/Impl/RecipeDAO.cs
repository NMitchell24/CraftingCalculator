using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using IngredientEntity = CraftingCalculator.Domain.Entities.Ingredient;
using RecipeChildEntity = CraftingCalculator.Domain.Entities.RecipeChild;
using RecipeEntity = CraftingCalculator.Domain.Entities.Recipe;
using RecipeFilterEntity = CraftingCalculator.Domain.Entities.RecipeFilter;
using RecipeIngredientEntity = CraftingCalculator.Domain.Entities.RecipeIngredient;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class RecipeDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IRecipeDAO
{
    public async Task<List<Recipe>> GetByFilterAsync(RecipeFilter filter)
    {
        RecipeGraph graph = await LoadGraphAsync();

        IEnumerable<RecipeEntity> matching = filter.Name == RecipeFilter.ALL
            ? graph.RecipesById.Values
            : graph.RecipesById.Values.Where(r => r.FilterId == filter.Id);

        return [.. matching.OrderBy(r => r.Name).Select(r => BuildModel(r, graph, 0))];
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        RecipeGraph graph = await LoadGraphAsync();

        return graph.RecipesById.TryGetValue(id, out RecipeEntity? entity) ? BuildModel(entity, graph, 0) : null;
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        RecipeGraph graph = await LoadGraphAsync();

        return [.. graph.RecipesById.Values.OrderBy(r => r.Name).Select(r => BuildModel(r, graph, 0))];
    }

    public async Task<Recipe> SaveAsync(Recipe recipe)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        RecipeEntity entity = recipe.Id > 0
            ? await context.Recipes.Include(r => r.Ingredients).FirstAsync(r => r.Id == recipe.Id)
            : new RecipeEntity();

        entity.Name = recipe.Name ?? "";
        entity.Description = recipe.Description ?? "";
        entity.Value = recipe.Value;
        entity.FilterId = recipe.Filter?.Id;

        if (entity.Id == 0)
        {
            context.Recipes.Add(entity);
        }

        foreach (IngredientQuantity removed in recipe.Ingredients.RemovedIngredients.Where(i => i.Id > 0))
        {
            RecipeIngredientEntity? toRemove = entity.Ingredients.FirstOrDefault(ri => ri.Id == removed.Id);
            if (toRemove != null)
            {
                context.RecipeIngredients.Remove(toRemove);
                entity.Ingredients.Remove(toRemove);
            }
        }

        foreach (IngredientQuantity iq in recipe.Ingredients.IngredientList)
        {
            if (iq.Id > 0)
            {
                entity.Ingredients.First(ri => ri.Id == iq.Id).Quantity = iq.Quantity;
            }
            else
            {
                entity.Ingredients.Add(new RecipeIngredientEntity { IngredientId = iq.Ingredient.Id, Quantity = iq.Quantity });
            }
        }

        // Save the parent recipe now: a newly-added child RecipeChild row below needs the parent's
        // (possibly newly-assigned) Id.
        await context.SaveChangesAsync();

        foreach (RecipeQuantity removed in recipe.ChildRecipes.RemovedRecipes.Where(r => r.Id > 0))
        {
            await context.RecipeChildren.Where(rc => rc.Id == removed.Id).ExecuteDeleteAsync();
        }

        foreach (RecipeQuantity rq in recipe.ChildRecipes.RecipeList)
        {
            if (rq.Id > 0)
            {
                RecipeChildEntity existing = await context.RecipeChildren.FirstAsync(rc => rc.Id == rq.Id);
                existing.Quantity = rq.Quantity;
            }
            else
            {
                context.RecipeChildren.Add(new RecipeChildEntity
                {
                    ParentRecipeId = entity.Id,
                    ChildRecipeId = rq.Recipe.Id,
                    Quantity = rq.Quantity
                });
            }
        }

        await context.SaveChangesAsync();

        recipe.Id = entity.Id;
        return recipe;
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Recipes.Where(r => r.Id == id).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Loads every recipe, ingredient, filter, and component link in five queries so the component
    /// graph can be stitched together in memory instead of one query per node (the shape the old
    /// LiteDB-backed recursive loader used, which the plan calls out as the wrong fit for EF Core).
    /// </summary>
    private async Task<RecipeGraph> LoadGraphAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dictionary<int, IngredientEntity> ingredientsById =
            await context.Ingredients.AsNoTracking().ToDictionaryAsync(i => i.Id);
        Dictionary<int, RecipeFilterEntity> filtersById =
            await context.RecipeFilters.AsNoTracking().ToDictionaryAsync(f => f.Id);
        Dictionary<int, RecipeEntity> recipesById =
            await context.Recipes.AsNoTracking().ToDictionaryAsync(r => r.Id);
        ILookup<int, RecipeIngredientEntity> ingredientsByRecipeId =
            (await context.RecipeIngredients.AsNoTracking().ToListAsync()).ToLookup(ri => ri.RecipeId);
        ILookup<int, RecipeChildEntity> childrenByParentId =
            (await context.RecipeChildren.AsNoTracking().ToListAsync()).ToLookup(rc => rc.ParentRecipeId);

        return new RecipeGraph(recipesById, ingredientsById, filtersById, ingredientsByRecipeId, childrenByParentId);
    }

    /// <summary>
    /// Recursively hydrates a full <see cref="Recipe"/> model (ingredients and, recursively, child
    /// recipes) from the in-memory graph. The app does not otherwise detect a cycle in the recipe
    /// graph (see the "no cycle guard" parity issue), so an A -> B -> A pair would recurse
    /// indefinitely; the shared <see cref="RecipeProcessor.MaxRecipeDepth"/> bound turns that into a
    /// catchable exception instead of a StackOverflowException.
    /// </summary>
    private static Recipe BuildModel(RecipeEntity entity, RecipeGraph graph, int depth)
    {
        if (depth > RecipeProcessor.MaxRecipeDepth)
        {
            throw new InvalidOperationException(
                $"Recipe graph exceeded the maximum depth of {RecipeProcessor.MaxRecipeDepth}; check for a cycle involving '{entity.Name}'.");
        }

        Recipe model = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Value = entity.Value
        };

        if (entity.FilterId is int filterId && graph.FiltersById.TryGetValue(filterId, out RecipeFilterEntity? filterEntity))
        {
            model.Filter = ToFilterModel(filterEntity);
        }

        foreach (RecipeIngredientEntity ri in graph.IngredientsByRecipeId[entity.Id])
        {
            if (graph.IngredientsById.TryGetValue(ri.IngredientId, out IngredientEntity? ingredientEntity))
            {
                model.Ingredients.Add(ToIngredientModel(ingredientEntity), ri.Quantity, ri.Id);
            }
        }

        foreach (RecipeChildEntity rc in graph.ChildrenByParentId[entity.Id])
        {
            if (graph.RecipesById.TryGetValue(rc.ChildRecipeId, out RecipeEntity? childEntity))
            {
                model.ChildRecipes.Add(BuildModel(childEntity, graph, depth + 1), rc.Quantity, rc.Id);
            }
        }

        return model;
    }

    private static RecipeFilter ToFilterModel(RecipeFilterEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };

    private static Ingredient ToIngredientModel(IngredientEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost
    };

    private sealed record RecipeGraph(
        Dictionary<int, RecipeEntity> RecipesById,
        Dictionary<int, IngredientEntity> IngredientsById,
        Dictionary<int, RecipeFilterEntity> FiltersById,
        ILookup<int, RecipeIngredientEntity> IngredientsByRecipeId,
        ILookup<int, RecipeChildEntity> ChildrenByParentId);
}
