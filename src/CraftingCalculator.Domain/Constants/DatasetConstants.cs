namespace CraftingCalculator.Domain.Constants;

public static class DatasetConstants
{
    /// <summary>
    /// The name the AddDatasets migration gives the dataset it files every pre-existing record into,
    /// and the name a dataset created from scratch gets. Users are free to rename it.
    /// </summary>
    public const string DefaultName = "Default";

    /// <summary>
    /// Appended to the name of a copied record or dataset. It sorts the copy next to what it was made from in any
    /// list ordered by name.
    /// </summary>
    public const string CopySuffix = " - Copy";
}
