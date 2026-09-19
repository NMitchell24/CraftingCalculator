namespace CraftingCalculator.Application.BusinessLogic.Transfer.Format;

// These types read every format version ever shipped, so a change here has to keep an older file deserializing.
// The rule: a member is only ever added, and it gets a default that stands in for what an older file does not
// say. If TransferFormat.CurrentVersion has shipped (it equals LatestShippedVersion), bump it and check a new
// export in under Fixtures/v{n}; if it has not, regenerate the current fixtures in place. A change that is not
// additive (a rename, a removal, a changed meaning) to a shipped version is upgraded from raw JSON in
// TransferDocumentReader before deserialization, so these types only ever describe the newest version. Never
// edit or delete a shipped version's fixture. The full procedure, with examples: docs/transfer-format-maintenance.md.

/// <summary>
/// A whole export file. Every <c>Ref</c> is local to the file and unique within its list, and every link
/// names a <c>Ref</c> in the list for its kind. <see cref="Datasettings"/> is null in a file that predates them,
/// which stands for the settings a new dataset starts with.
/// </summary>
public sealed record TransferDocument(
    string Format,
    int FormatVersion,
    DateTimeOffset ExportedAt,
    string AppVersion,
    string DatasetName,
    IReadOnlyList<TransferCategory> Categories,
    IReadOnlyList<TransferComponent> Components,
    IReadOnlyList<TransferBlueprint> Blueprints,
    IReadOnlyList<TransferFavorite> Favorites,
    TransferDatasettings? Datasettings = null);

/// <summary>The dataset's own settings. Each member's default is the setting's default.</summary>
public sealed record TransferDatasettings(
    bool UseYield = true,
    bool UseCosts = true,
    bool UseValues = true,
    bool UseCraftTime = true,
    string? CurrencyLabel = null);

public sealed record TransferCategory(int Ref, string Name, string Description);

/// <summary>A component. <see cref="Category"/> is a category's <c>Ref</c>, or null.</summary>
public sealed record TransferComponent(
    int Ref, string Name, string Description, double Cost, TimeSpan ProductionTime, int? Category);

/// <summary>
/// A blueprint. <see cref="Category"/> is a category's <c>Ref</c>, or null; <see cref="Components"/> name
/// components and <see cref="Blueprints"/> the blueprints nested inside it.
/// </summary>
public sealed record TransferBlueprint(
    int Ref,
    string Name,
    string Description,
    double Value,
    long Yield,
    TimeSpan ProductionTime,
    int? Category,
    IReadOnlyList<QuantityRef> Components,
    IReadOnlyList<QuantityRef> Blueprints);

/// <summary>A favorite. <see cref="Blueprints"/> name blueprints.</summary>
public sealed record TransferFavorite(int Ref, string Name, IReadOnlyList<QuantityRef> Blueprints);

/// <summary>A quantity of the record with <see cref="Ref"/>.</summary>
public sealed record QuantityRef(int Ref, long Quantity);
