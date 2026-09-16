using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The collection a <see cref="RecordPickerDialog" /> adds records to and changes quantities on. Each change applies
/// the moment the user makes it.
/// </summary>
public interface IRecordPickerTarget
{
    /// <summary>The entry <paramref name="record" /> already has in the collection, or <c>null</c> when it has none.</summary>
    IBaseQuantityRecord? Find(IBaseDataRecord record);

    /// <summary>Adds one of <paramref name="record" /> to the collection.</summary>
    Task AddAsync(IBaseDataRecord record);

    /// <summary>Moves the entry's quantity by <paramref name="step" />, which is negative to step down.</summary>
    Task StepAsync(IBaseQuantityRecord entry, long step);

    /// <summary>Sets the entry's quantity to <paramref name="quantity" />.</summary>
    Task SetQuantityAsync(IBaseQuantityRecord entry, long quantity);
}
