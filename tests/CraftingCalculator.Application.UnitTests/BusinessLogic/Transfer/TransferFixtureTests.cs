using System.Text.Json;
using System.Text.Json.Nodes;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

/// <summary>
/// The backward-compatibility tripwire. Every file under <c>Fixtures/v{n}</c> is a real export from format
/// version n, and has to stay readable for as long as the app exists. A fixture of a version up to
/// <see cref="TransferFormat.LatestShippedVersion"/> is never edited or deleted: a test failing on one means a change
/// broke files people already have. The current version's fixtures are the writer's own output, so a change to the
/// file fails here until they are regenerated, and until the next release that is all a change costs
/// (docs/transfer-format-maintenance.md).
/// </summary>
[TestFixture]
public class TransferFixtureTests
{
    private static readonly string FixturesRoot =
        Path.Combine(TestContext.CurrentContext.TestDirectory, "BusinessLogic", "Transfer", "Fixtures");

    private static readonly string CurrentBronzeChain =
        Path.Combine($"v{TransferFormat.CurrentVersion}", "bronze-chain.ccdata");

    private static IEnumerable<string> Fixtures() =>
        Directory.EnumerateFiles(FixturesRoot, $"*{TransferFormat.FileExtension}", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(FixturesRoot, path));

    [Test]
    public void TheFormatVersion_IsBumpedAtMostOncePerRelease() =>
        TransferFormat.CurrentVersion.Should().BeInRange(
            TransferFormat.LatestShippedVersion, TransferFormat.LatestShippedVersion + 1,
            "a format change before the next release regenerates Fixtures/v{0} instead of bumping again "
            + "(docs/transfer-format-maintenance.md)", TransferFormat.CurrentVersion);

    [Test]
    public void TheCurrentFormatVersion_HasAFixture() =>
        Directory.Exists(Path.Combine(FixturesRoot, $"v{TransferFormat.CurrentVersion}"))
            .Should().BeTrue("every format version the app writes needs a checked-in export to keep reading");

    [TestCaseSource(nameof(Fixtures))]
    public void EveryFixture_DeclaresTheFormatVersionItsFolderNames(string fixture)
    {
        // Peeked from the JSON rather than read through the document types: a fixture is pinned to the version it
        // was exported as, whatever the types have gained since.
        using FileStream stream = File.OpenRead(Path.Combine(FixturesRoot, fixture));
        using JsonDocument json = JsonDocument.Parse(stream);

        json.RootElement.GetProperty("format").GetString().Should().Be(TransferFormat.Name);
        $"v{json.RootElement.GetProperty("formatVersion").GetInt32()}".Should().Be(Path.GetDirectoryName(fixture));
    }

    [TestCaseSource(nameof(Fixtures))]
    public void EveryFixture_PassesImportValidation(string fixture)
    {
        using FileStream stream = File.OpenRead(Path.Combine(FixturesRoot, fixture));

        ImportValidationResult result = TransferDocumentReader.Read(stream);

        // Infrastructure's TransferFixtureImportTests writes each one to a database as well.
        result.Errors.Should().BeEmpty();
        result.File.Should().NotBeNull();
    }

    [Test]
    public void TheCurrentBronzeChainFixture_IsWhatTheWriterProducesForTheBronzeChain()
    {
        // Compared as JSON, not as records: a member added to the document types reads back from an older fixture
        // with its default filled in, so only the file itself shows that the fixture predates the member.
        JsonNode file = JsonNode.Parse(File.ReadAllText(Path.Combine(FixturesRoot, CurrentBronzeChain)))!;

        JsonNode.DeepEquals(file, JsonSerializer.SerializeToNode(BronzeChainExport(), TransferJsonContext.Default.TransferDocument))
            .Should().BeTrue($"the fixture under v{TransferFormat.CurrentVersion} has to be exactly what this build writes: "
                             + "regenerate it with WriteTheCurrentBronzeChainFixture, after bumping the version first "
                             + "if this one has shipped (docs/transfer-format-maintenance.md)");
    }

    /// <summary>
    /// Writes the current version's bronze chain fixture into the source tree. Run it whenever the file the app
    /// writes changes, then check the file in; never against a version that has shipped.
    /// </summary>
    [Test]
    [Explicit("regenerates a checked-in fixture")]
    public void WriteTheCurrentBronzeChainFixture()
    {
        // The test directory is bin/<Configuration>/<TFM>; three up is the project, where the fixtures are sourced.
        string source = Path.GetFullPath(Path.Combine(
            TestContext.CurrentContext.TestDirectory, "..", "..", "..", "BusinessLogic", "Transfer", "Fixtures", CurrentBronzeChain));

        Directory.CreateDirectory(Path.GetDirectoryName(source)!);
        // WriteIndented follows Environment.NewLine; the checked-in fixtures are LF.
        string json = JsonSerializer.Serialize(BronzeChainExport(), TransferJsonContext.Default.TransferDocument);

        //Assertion is needed to satisfy SonarLint rules.
        json.Should().NotBeNullOrEmpty("the export should never be empty, or the fixture would be useless");

        File.WriteAllText(source, json.ReplaceLineEndings("\n") + "\n");

        TestContext.Out.WriteLine($"Wrote {source}");
    }

    /// <summary>The whole bronze chain as this build exports it, which is what the current version's fixture holds.</summary>
    private static TransferDocument BronzeChainExport()
    {
        HashSet<RecordKey> everything =
        [
            BronzeChain.Metals, BronzeChain.Tools, BronzeChain.Food, BronzeChain.Copper, BronzeChain.Tin,
            BronzeChain.Wood, BronzeChain.Bronze, BronzeChain.BronzeAxe, BronzeChain.BronzeNails,
            BronzeChain.AxeRun, BronzeChain.KarvePrep
        ];

        // Fixed inputs, not the clock or the build's version, so the fixture is reproducible.
        return TransferDocumentProcessor.ToDocument(
            BronzeChain.Snapshot, everything, new DateTimeOffset(2026, 9, 12, 18, 4, 0, TimeSpan.Zero), "2.0");
    }

    [Test]
    public void AWrittenDocument_ReadsBackUnchanged()
    {
        TransferDocument original = Read(CurrentBronzeChain);

        string json = JsonSerializer.Serialize(original, TransferJsonContext.Default.TransferDocument);

        JsonSerializer.Deserialize(json, TransferJsonContext.Default.TransferDocument).Should().BeEquivalentTo(original);
    }

    private static TransferDocument Read(string fixture)
    {
        using FileStream stream = File.OpenRead(Path.Combine(FixturesRoot, fixture));

        return JsonSerializer.Deserialize(stream, TransferJsonContext.Default.TransferDocument)
               ?? throw new InvalidDataException($"{fixture} is empty.");
    }
}
