using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;
using static CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer.BronzeChain;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

[TestFixture]
public class TransferDocumentProcessorTests
{
    private static readonly DateTimeOffset ExportedAt = new(2026, 9, 12, 18, 4, 0, TimeSpan.Zero);

    // The nails' chain and the unused Food category: every kind skips an id, so a ref that simply copied
    // the database id would show up as wrong.
    private static readonly HashSet<RecordKey> NailsOnly = [Metals, Food, Copper, Tin, Bronze, BronzeNails];

    [Test]
    public void ToDocument_WritesTheHeader()
    {
        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, NailsOnly, ExportedAt, "1.0");

        document.Format.Should().Be(TransferFormat.Name);
        document.FormatVersion.Should().Be(TransferFormat.CurrentVersion);
        document.ExportedAt.Should().Be(ExportedAt);
        document.AppVersion.Should().Be("1.0");
        document.DatasetName.Should().Be("Valheim");
    }

    [TestCase(false, false, true)]
    [TestCase(true, false, false)]
    public void ToDocument_WritesTheDatasettingsWhateverIsSelected(bool useYield, bool useCosts, bool useValues)
    {
        DatasetSnapshot snapshot =
            Snapshot with { Settings = new Datasettings(UseYield: useYield, UseCosts: useCosts, UseValues: useValues) };

        TransferDocumentProcessor.ToDocument(snapshot, new HashSet<RecordKey>(), ExportedAt, "1.0")
            .Datasettings.Should().Be(new TransferDatasettings(UseYield: useYield, UseCosts: useCosts, UseValues: useValues));
    }

    [Test]
    public void ToDocument_RecordsTheExportTimeInUtc()
    {
        DateTimeOffset local = new(2026, 9, 12, 13, 4, 0, TimeSpan.FromHours(-5));

        TransferDocumentProcessor.ToDocument(Snapshot, NailsOnly, local, "1.0").ExportedAt.Offset.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void ToDocument_LeavesOutWhatIsNotSelected()
    {
        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, NailsOnly, ExportedAt, "1.0");

        document.Categories.Select(category => category.Name).Should().Equal("Metals", "Food");
        document.Components.Select(component => component.Name).Should().Equal("Copper", "Tin");
        document.Blueprints.Select(blueprint => blueprint.Name).Should().Equal("Bronze", "Bronze Nails");
        document.Favorites.Should().BeEmpty();
    }

    [Test]
    public void ToDocument_NumbersRefsFromOnePerKind()
    {
        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, NailsOnly, ExportedAt, "1.0");

        document.Categories.Select(category => category.Ref).Should().Equal(1, 2);
        document.Blueprints.Select(blueprint => blueprint.Ref).Should().Equal(1, 2);
    }

    [Test]
    public void ToDocument_PointsEveryLinkAtARef()
    {
        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, NailsOnly, ExportedAt, "1.0");

        TransferBlueprint bronze = document.Blueprints.Single(blueprint => blueprint.Name == "Bronze");
        TransferBlueprint nails = document.Blueprints.Single(blueprint => blueprint.Name == "Bronze Nails");

        // Bronze Nails is blueprint id 3 in the snapshot and ref 2 in the file.
        nails.Ref.Should().Be(2);
        nails.Blueprints.Should().Equal(new QuantityRef(bronze.Ref, 1));
        bronze.Components.Should().Equal(new QuantityRef(1, 2), new QuantityRef(2, 1));
    }

    [Test]
    public void ToDocument_PointsACategoryAtItsRefAndLeavesAnUncategorizedRecordNull()
    {
        HashSet<RecordKey> selected = [Food, Wood, Metals, Copper];

        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, selected, ExportedAt, "1.0");

        // Metals is category id 1 but comes after nothing selected before it, so ref 1; Food is id 3, ref 2.
        document.Components.Single(component => component.Name == "Copper").Category.Should().Be(1);
        document.Components.Single(component => component.Name == "Wood").Category.Should().BeNull();
    }

    [Test]
    public void ToDocument_CopiesEveryField()
    {
        HashSet<RecordKey> selected = [Metals, Copper];

        TransferDocument document = TransferDocumentProcessor.ToDocument(Snapshot, selected, ExportedAt, "1.0");

        document.Categories.Single().Should().Be(new TransferCategory(1, "Metals", "Smelted in the furnace"));
        document.Components.Single().Should().Be(new TransferComponent(1, "Copper", "Ore", 2, TimeSpan.FromSeconds(30), 1));
    }

    [Test]
    public void ToDocument_ASelectionMissingADependency_Throws()
    {
        // Bronze without the Tin it is made from.
        HashSet<RecordKey> selected = [Metals, Copper, Bronze];

        Action act = () => TransferDocumentProcessor.ToDocument(Snapshot, selected, ExportedAt, "1.0");

        act.Should().Throw<InvalidOperationException>();
    }
}
