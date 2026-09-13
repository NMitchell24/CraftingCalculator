using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// Fails when an entity that export files carry gains, loses or renames a column. The export format is
/// frozen per version, so a schema change has to be a decision: does the file need a new format version,
/// or is the new column deliberately left out of it? Make that call, then update the list here.
/// </summary>
[TestFixture]
public class TransferSchemaTripwireTests
{
    private const string Decide =
        "an exported entity changed: decide whether the export format needs a new version, then update this list";

    private static readonly (Type Entity, string[] Properties)[] Pinned =
    [
        (typeof(Category), ["Id", "Name", "Description", "DatasetId"]),
        (typeof(Component), ["Id", "Name", "Description", "Cost", "ProductionTime", "DatasetId", "CategoryId"]),
        (typeof(Blueprint), ["Id", "Name", "Description", "Value", "Yield", "ProductionTime", "DatasetId", "CategoryId"]),
        (typeof(Favorite), ["Id", "Name", "DatasetId"]),
        (typeof(BlueprintComponent), ["Id", "BlueprintId", "ComponentId", "Quantity"]),
        (typeof(BlueprintChild), ["Id", "ParentBlueprintId", "ChildBlueprintId", "Quantity"]),
        (typeof(FavoriteBlueprint), ["Id", "FavoriteId", "BlueprintId", "Quantity"])
    ];

    private static IEnumerable<TestCaseData> Entities() =>
        Pinned.Select(pinned => new TestCaseData(pinned.Entity, pinned.Properties).SetArgDisplayNames(pinned.Entity.Name));

    [TestCaseSource(nameof(Entities))]
    public void AnExportedEntity_HasThePinnedProperties(Type entity, string[] properties)
    {
        using SqliteTestFixture fixture = new();
        using CraftingDataContext context = fixture.RawFactory.CreateDbContext();

        IEntityType type = context.Model.FindEntityType(entity)!;

        type.GetProperties().Select(property => property.Name).Should().BeEquivalentTo(properties, Decide);
    }
}
