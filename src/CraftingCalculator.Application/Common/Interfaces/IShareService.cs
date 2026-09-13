namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the platform share sheet. Implemented in the UI project against
/// Microsoft.Maui.ApplicationModel.DataTransfer.Share so Application has no MAUI dependency.
/// </summary>
public interface IShareService
{
    /// <summary>Opens the share sheet for the file at <paramref name="path"/>, titled <paramref name="title"/>.</summary>
    Task ShareFileAsync(string path, string title);
}
