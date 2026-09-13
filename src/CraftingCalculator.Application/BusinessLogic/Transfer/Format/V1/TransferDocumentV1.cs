namespace CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;

// FROZEN. Version 1 of the export file has shipped once these are released: never rename, reorder, add or
// remove a member here, and never edit a fixture under Fixtures/v1. A change the file has to carry is a new
// Format/V2 set of types plus an upgrader from this one. TransferSchemaTripwireTests fails when an exported
// entity changes, which is the prompt to decide that.

/// <summary>
/// A whole version 1 export file. Every <c>Ref</c> is local to the file and unique within its list, and
/// every link names a <c>Ref</c> in the list for its kind.
/// </summary>
public sealed record TransferDocumentV1(
    string Format,
    int FormatVersion,
    DateTimeOffset ExportedAt,
    string AppVersion,
    string DatasetName,
    IReadOnlyList<CategoryV1> Categories,
    IReadOnlyList<ComponentV1> Components,
    IReadOnlyList<BlueprintV1> Blueprints,
    IReadOnlyList<FavoriteV1> Favorites);

public sealed record CategoryV1(int Ref, string Name, string Description);

/// <summary>A component. <see cref="Category"/> is a category's <c>Ref</c>, or null.</summary>
public sealed record ComponentV1(
    int Ref, string Name, string Description, double Cost, TimeSpan ProductionTime, int? Category);

/// <summary>
/// A blueprint. <see cref="Category"/> is a category's <c>Ref</c>, or null; <see cref="Components"/> name
/// components and <see cref="Blueprints"/> the blueprints nested inside it.
/// </summary>
public sealed record BlueprintV1(
    int Ref,
    string Name,
    string Description,
    double Value,
    long Yield,
    TimeSpan ProductionTime,
    int? Category,
    IReadOnlyList<QuantityRefV1> Components,
    IReadOnlyList<QuantityRefV1> Blueprints);

/// <summary>A favorite. <see cref="Blueprints"/> name blueprints.</summary>
public sealed record FavoriteV1(int Ref, string Name, IReadOnlyList<QuantityRefV1> Blueprints);

/// <summary>A quantity of the record with <see cref="Ref"/>.</summary>
public sealed record QuantityRefV1(int Ref, long Quantity);
