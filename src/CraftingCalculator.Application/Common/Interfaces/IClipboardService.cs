namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Abstraction over the platform clipboard. Implemented in the UI project against
/// Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard so Application has no MAUI dependency.
/// </summary>
public interface IClipboardService
{
    Task SetTextAsync(string text);
}
