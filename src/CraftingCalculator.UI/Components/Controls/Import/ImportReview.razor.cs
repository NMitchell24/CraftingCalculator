using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls.Import;

/// <summary>
/// The import wizard's second step: which dataset the file holds, when and by which version of the app it was exported,
/// and the selection panels for choosing which of its records to import.
/// </summary>
public partial class ImportReview : BaseImportControl
{
    /// <summary>Raised when the user taps Import Data.</summary>
    [Parameter] public EventCallback OnImport { get; set; }

    /// <summary>Raised after every change the selection panels make to the selection.</summary>
    [Parameter] public EventCallback SelectionChanged { get; set; }
}
