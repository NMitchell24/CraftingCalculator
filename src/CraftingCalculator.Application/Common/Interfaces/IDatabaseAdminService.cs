namespace CraftingCalculator.Application.Common.Interfaces;

public interface IDatabaseAdminService
{
    /// <summary>Deletes all data from the selected dataset, leaving it empty.</summary>
    Task DeleteAllDataAsync();
}
