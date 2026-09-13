using System.Text.Json;
using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

/// <summary>
/// The backward-compatibility tripwire. Every file under <c>Fixtures/v{n}</c> is a real export from format
/// version n, and has to stay readable for as long as the app exists. Never edit or delete a fixture: a test
/// failing here means a change broke files people already have.
/// </summary>
[TestFixture]
public class TransferFixtureTests
{
    private static readonly string FixturesRoot =
        Path.Combine(TestContext.CurrentContext.TestDirectory, "BusinessLogic", "Transfer", "Fixtures");

    private static IEnumerable<string> Fixtures() =>
        Directory.EnumerateFiles(FixturesRoot, $"*{TransferFormat.FileExtension}", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(FixturesRoot, path));

    [Test]
    public void TheCurrentFormatVersion_HasAFixture() =>
        Directory.Exists(Path.Combine(FixturesRoot, $"v{TransferFormat.CurrentVersion}"))
            .Should().BeTrue("every format version the app writes needs a checked-in export to keep reading");

    [TestCaseSource(nameof(Fixtures))]
    public void EveryFixture_ReadsAsTheFormatVersionItsFolderNames(string fixture)
    {
        // Only version 1 exists. When version 2 arrives this reads through the upgrader chain instead.
        TransferDocumentV1 document = Read(fixture);

        document.Format.Should().Be(TransferFormat.Name);
        $"v{document.FormatVersion}".Should().Be(Path.GetDirectoryName(fixture));
    }

    [Test]
    public void TheBronzeChainFixture_IsWhatTheWriterProducesForTheBronzeChain()
    {
        HashSet<RecordKey> everything =
        [
            BronzeChain.Metals, BronzeChain.Tools, BronzeChain.Food, BronzeChain.Copper, BronzeChain.Tin,
            BronzeChain.Wood, BronzeChain.Bronze, BronzeChain.BronzeAxe, BronzeChain.BronzeNails,
            BronzeChain.AxeRun, BronzeChain.KarvePrep
        ];

        TransferDocumentV1 written = TransferDocumentProcessor.ToDocument(
            BronzeChain.Snapshot, everything, new DateTimeOffset(2026, 9, 12, 18, 4, 0, TimeSpan.Zero), "1.0");

        Read(Path.Combine("v1", "bronze-chain.ccdata")).Should().BeEquivalentTo(written);
    }

    [Test]
    public void AWrittenDocument_ReadsBackUnchanged()
    {
        TransferDocumentV1 original = Read(Path.Combine("v1", "bronze-chain.ccdata"));

        string json = JsonSerializer.Serialize(original, TransferJsonContext.Default.TransferDocumentV1);

        JsonSerializer.Deserialize(json, TransferJsonContext.Default.TransferDocumentV1).Should().BeEquivalentTo(original);
    }

    private static TransferDocumentV1 Read(string fixture)
    {
        using FileStream stream = File.OpenRead(Path.Combine(FixturesRoot, fixture));

        return JsonSerializer.Deserialize(stream, TransferJsonContext.Default.TransferDocumentV1)
               ?? throw new InvalidDataException($"{fixture} is empty.");
    }
}
