using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Domain.Constants;

/// <summary>Values baked into the database by the initial seed migration.</summary>
public static class DatabaseSeedConstants
{
    /// <summary>Id of the seeded <see cref="CategoryModel.ALL"/> category row.</summary>
    public const int AllCategoryId = 1;
}
