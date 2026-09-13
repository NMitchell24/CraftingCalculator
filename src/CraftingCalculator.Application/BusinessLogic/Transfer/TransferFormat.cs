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

    // Platforms/iOS/Info.plist declares the same identifier. It is derived from the app id, so the app id rename
    // before release changes it in both places.
    /// <summary>The uniform type identifier iOS knows an export file by.</summary>
    public const string UniformTypeIdentifier = "com.nathanmitchell.craftingcalculator.ccdata";
}
