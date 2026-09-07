using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

public class ClipboardService : IClipboardService
{
    public Task SetTextAsync(string text) => Clipboard.Default.SetTextAsync(text);
}
