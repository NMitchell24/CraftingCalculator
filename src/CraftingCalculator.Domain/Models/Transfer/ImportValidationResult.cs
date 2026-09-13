namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// What reading an import file found: the file's records and export details when it can be imported, or else every
/// reason it can't, in words for the user. <see cref="File"/> is null exactly when <see cref="Errors"/> is not empty.
/// </summary>
public sealed record ImportValidationResult(ImportFile? File, IReadOnlyList<string> Errors);
