namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the platform file picker. Implemented in the UI project against
/// Microsoft.Maui.Storage.FilePicker so Application has no MAUI dependency.
/// </summary>
public interface IImportFilePicker
{
    /// <summary>
    /// Lets the user choose a file to import and returns the path of the app's own copy of it, or null when they
    /// backed out. Each pick replaces the previous copy. A copy stops one byte past
    /// <see cref="BusinessLogic.Transfer.TransferDocumentReader.MaxFileBytes"/>.
    /// </summary>
    Task<string?> PickAsync();
}
