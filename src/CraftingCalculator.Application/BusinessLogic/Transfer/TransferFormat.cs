namespace CraftingCalculator.Application.BusinessLogic.Transfer;

/// <summary>What identifies a Crafting Calculator export file.</summary>
public static class TransferFormat
{
    /// <summary>The <c>format</c> value at the top of every export file.</summary>
    public const string Name = "crafting-calculator-dataset";

    /// <summary>The <c>formatVersion</c> this build of the app writes.</summary>
    public const int CurrentVersion = 1;

    /// <summary>The extension every export file is saved with, dot included.</summary>
    public const string FileExtension = ".ccdata";
}
