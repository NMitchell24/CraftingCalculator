using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

/// <summary>
/// Every way an import file can be turned away. Each test starts from the bronze chain as the writer exports it and
/// breaks one thing, so a failure names exactly the check that let it through.
/// </summary>
[TestFixture]
public class TransferDocumentReaderTests
{
    // BlueprintProcessor.MaxBlueprintDepth, which is internal to the Application assembly.
    private const int MaxBlueprintDepth = 64;

    private static TransferDocument BronzeChainDocument() =>
        TransferDocumentProcessor.ToDocument(
            BronzeChain.Snapshot,
            new HashSet<RecordKey>(DependencyGraphProcessor.Build(BronzeChain.Snapshot).All),
            new DateTimeOffset(2026, 9, 12, 18, 4, 0, TimeSpan.Zero),
            "1.0");

    private static ImportValidationResult Read(string json)
    {
        using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));
        return TransferDocumentReader.Read(stream);
    }

    private static ImportValidationResult Read(TransferDocument document) =>
        Read(JsonSerializer.Serialize(document, TransferJsonContext.Default.TransferDocument));

    /// <summary>Reads the bronze chain export after <paramref name="edit"/> has changed its JSON.</summary>
    private static ImportValidationResult ReadEdited(Action<JsonObject> edit)
    {
        JsonObject root = JsonSerializer.SerializeToNode(BronzeChainDocument(), TransferJsonContext.Default.TransferDocument)!
            .AsObject();
        edit(root);

        return Read(root.ToJsonString());
    }

    private static JsonObject Record(JsonObject root, string list, int index) => root[list]![index]!.AsObject();

    /// <summary>A document holding only a chain of <paramref name="length"/> blueprints, each nesting the next.</summary>
    private static TransferDocument ChainOf(int length) => new(
        TransferFormat.Name, TransferFormat.CurrentVersion, DateTimeOffset.UnixEpoch, "1.0", "Deep", [], [],
        [
            .. Enumerable.Range(1, length).Select(reference => new TransferBlueprint(
                reference, $"Tier {reference}", "", 0, 1, TimeSpan.Zero, null, [],
                reference < length ? [new QuantityRef(reference + 1, 1)] : []))
        ],
        []);

    private static void ShouldBeRejectedWith(ImportValidationResult result, string fragment)
    {
        result.File.Should().BeNull();
        result.Errors.Should().Contain(error => error.Contains(fragment));
    }

    [Test]
    public void Read_AnExport_ReturnsItsRecordsWithRefsAsIds()
    {
        ImportValidationResult result = Read(BronzeChainDocument());

        // The bronze chain's ids already run 1 up in list order, which is exactly how the writer numbers refs.
        result.Errors.Should().BeEmpty();
        result.File!.Snapshot.Should().BeEquivalentTo(BronzeChain.Snapshot);
    }

    [Test]
    public void Read_AnExport_ReturnsWhenAndByWhichVersionItWasExported()
    {
        ImportValidationResult result = Read(BronzeChainDocument());

        result.File!.ExportedAt.Should().Be(new DateTimeOffset(2026, 9, 12, 18, 4, 0, TimeSpan.Zero));
        result.File.AppVersion.Should().Be("1.0");
    }

    [Test]
    public void Read_AFileOverTheSizeLimit_IsRejectedWithoutBeingRead()
    {
        using LengthOnlyStream stream = new(TransferDocumentReader.MaxFileBytes + 1);

        ShouldBeRejectedWith(TransferDocumentReader.Read(stream), "bigger than");
    }

    [TestCase("")]
    [TestCase("this is a shopping list")]
    [TestCase("{\"format\": \"crafting-calculator-dataset\", ")]
    public void Read_SomethingThatIsNotJson_SaysTheFileIsDamaged(string content) =>
        ShouldBeRejectedWith(Read(content), "damaged");

    [Test]
    public void Read_AnExportCutOffPartway_SaysTheFileIsDamaged()
    {
        string json = JsonSerializer.Serialize(BronzeChainDocument(), TransferJsonContext.Default.TransferDocument);

        ShouldBeRejectedWith(Read(json[..(json.Length / 2)]), "damaged");
    }

    [Test]
    public void Read_NestingPastTheParserLimit_SaysTheFileIsDamaged() =>
        ShouldBeRejectedWith(Read(new string('[', 10_000) + new string(']', 10_000)), "damaged");

    [TestCase("[]")]
    [TestCase("{}")]
    [TestCase("{\"format\": \"minecraft-world\", \"formatVersion\": 1}")]
    [TestCase("{\"format\": \"crafting-calculator-dataset\"}")]
    [TestCase("{\"format\": \"crafting-calculator-dataset\", \"formatVersion\": \"1\"}")]
    [TestCase("{\"format\": \"crafting-calculator-dataset\", \"formatVersion\": 0}")]
    public void Read_JsonThatIsNotAnExport_SaysSo(string json) =>
        ShouldBeRejectedWith(Read(json), "isn't a Crafting Calculator export");

    [Test]
    public void Read_AFileFromANewerVersionOfTheApp_AsksForAnUpdate() =>
        ShouldBeRejectedWith(ReadEdited(root => root["formatVersion"] = 999), "Update the app");

    [Test]
    public void Read_AMemberAnExportDoesNotHave_IsRejected() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "components", 0)["$type"] = "System.IO.File"), "isn't laid out");

    [Test]
    public void Read_AMissingMember_IsRejected() =>
        ShouldBeRejectedWith(ReadEdited(root => root.Remove("favorites")), "isn't laid out");

    [Test]
    public void Read_ANullWhereAnExportNeverHasOne_IsRejected() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "categories", 0)["name"] = null), "isn't laid out");

    [Test]
    public void Read_ACostThatIsNotANumber_IsRejected() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "components", 0)["cost"] = "NaN"), "isn't laid out");

    [Test]
    public void Read_AnEmptyEntryInAList_IsRejected() =>
        ShouldBeRejectedWith(ReadEdited(root => root["components"]!.AsArray().Add(null)), "empty where a record should be");

    [Test]
    public void Read_TwoRecordsSharingARef_IsReported() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "components", 1)["ref"] = 1), "More than one component uses ref 1");

    [Test]
    public void Read_ARefBelowOne_IsReported() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "categories", 2)["ref"] = 0), "The category 'Food' has an invalid ref (0)");

    [Test]
    public void Read_AComponentFiledUnderAMissingCategory_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "components", 0)["category"] = 42),
            "The component 'Copper' is filed under a category that isn't in the file (ref 42)");

    [Test]
    public void Read_ABlueprintUsingAMissingComponent_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "blueprints", 0)["components"]![0]!["ref"] = 42),
            "The blueprint 'Bronze' uses a component that isn't in the file (ref 42)");

    [Test]
    public void Read_ABlueprintNestingAMissingBlueprint_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "blueprints", 1)["blueprints"]![0]!["ref"] = 42),
            "The blueprint 'Bronze Axe' uses a blueprint that isn't in the file (ref 42)");

    [Test]
    public void Read_AFavoriteHoldingAMissingBlueprint_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "favorites", 0)["blueprints"]![0]!["ref"] = 42),
            "The favorite 'Bronze Axe run' uses a blueprint that isn't in the file (ref 42)");

    [Test]
    public void Read_ARecordWithABlankName_IsReported() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "blueprints", 2)["name"] = "   "), "A blueprint has no name (ref 3)");

    [Test]
    public void Read_ANameLongerThanAnyoneTypes_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "categories", 0)["name"] = new string('M', 10_001)),
            "is longer than 10,000 characters");

    [Test]
    public void Read_AnAppVersionLongerThanAnyoneTypes_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => root["appVersion"] = new string('1', 10_001)),
            "The app version is longer than 10,000 characters");

    [Test]
    public void Read_ANegativeQuantity_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "blueprints", 0)["components"]![0]!["quantity"] = -2),
            "The blueprint 'Bronze' uses a negative quantity of a component");

    [Test]
    public void Read_AQuantityOfZero_IsAllowed()
    {
        // The blueprint editor and the Craft screen both let a quantity sit at zero, so an export can carry one.
        ImportValidationResult result = ReadEdited(root => Record(root, "favorites", 0)["blueprints"]![0]!["quantity"] = 0);

        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Read_AYieldBelowOne_IsReported() =>
        ShouldBeRejectedWith(ReadEdited(root => Record(root, "blueprints", 2)["yield"] = 0), "The blueprint 'Bronze Nails' makes fewer than 1 item");

    [Test]
    public void Read_ANegativeProductionTime_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "components", 0)["productionTime"] = "-00:00:30"),
            "The component 'Copper' has a negative production time");

    [Test]
    public void Read_MoreRecordsOfAKindThanAnyDataset_IsReported()
    {
        TransferDocument document = BronzeChainDocument() with
        {
            Components = [.. Enumerable.Range(1, 100_001).Select(reference => new TransferComponent(reference, "Stone", "", 0, TimeSpan.Zero, null))],
            Blueprints = [],
            Favorites = []
        };

        ShouldBeRejectedWith(Read(document), "more than 100,000 components");
    }

    [Test]
    public void Read_ABlueprintNestedInsideItself_IsReported() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "blueprints", 0)["blueprints"]!.AsArray().Add(new JsonObject { ["ref"] = 1, ["quantity"] = 1 })),
            "The blueprint 'Bronze' is nested inside itself");

    [Test]
    public void Read_TwoBlueprintsNestedInsideEachOther_NamesBoth() =>
        ShouldBeRejectedWith(
            ReadEdited(root => Record(root, "blueprints", 0)["blueprints"]!.AsArray().Add(new JsonObject { ["ref"] = 3, ["quantity"] = 1 })),
            "The blueprints 'Bronze' and 'Bronze Nails' are nested inside each other");

    [Test]
    public void Read_BlueprintsNestedAsDeepAsTheAppWorksOut_AreAllowed() =>
        Read(ChainOf(MaxBlueprintDepth + 1)).Errors.Should().BeEmpty();

    [Test]
    public void Read_BlueprintsNestedDeeperThanTheAppWorksOut_AreReported() =>
        ShouldBeRejectedWith(Read(ChainOf(MaxBlueprintDepth + 2)), "The blueprint 'Tier 1' has blueprints nested 65 levels deep");

    [Test]
    public void Read_AVeryLongChain_DoesNotOverflowTheStack() =>
        ShouldBeRejectedWith(Read(ChainOf(100_000)), "nested 99999 levels deep");

    [Test]
    public void Read_SeveralProblems_ListsEveryOne()
    {
        ImportValidationResult result = ReadEdited(root =>
        {
            Record(root, "blueprints", 2)["yield"] = 0;
            Record(root, "components", 0)["category"] = 42;
        });

        result.Errors.Should().HaveCount(2);
    }

    [Test]
    public void Read_MoreProblemsThanTheListShows_CountsTheRest()
    {
        TransferDocument document = BronzeChainDocument() with
        {
            Components = [.. Enumerable.Range(1, 60).Select(reference => new TransferComponent(reference, "", "", 0, TimeSpan.Zero, null))],
            Blueprints = [],
            Favorites = []
        };

        ImportValidationResult result = Read(document);

        result.Errors.Should().HaveCount(51);
        result.Errors[^1].Should().Be("…and 10 more problems.");
    }

    [Test]
    public void Read_MoreBadLinksThanTheListShows_CountsTheRest()
    {
        ImportValidationResult result = ReadEdited(root => Record(root, "blueprints", 0)["components"] = new JsonArray(
            Enumerable.Range(0, 1_000).Select(_ => (JsonNode?)new JsonObject { ["ref"] = 42, ["quantity"] = 1 }).ToArray()));

        result.Errors.Should().HaveCount(51);
        result.Errors[^1].Should().Be("…and 950 more problems.");
    }
}

/// <summary>A stream that reports a length and fails if anything tries to read it.</summary>
file sealed class LengthOnlyStream(long length) : Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => length;
    public override long Position { get; set; }

    public override int Read(byte[] buffer, int offset, int count) =>
        throw new AssertionException("The file was read even though it is over the size limit.");

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Flush() { }
}
