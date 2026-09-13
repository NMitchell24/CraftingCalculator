using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// The other half of the Application tests' TransferFixtureTests: every export file ever shipped has to import into a
/// real database, not just read. Never edit or delete a fixture to make this pass.
/// </summary>
[TestFixture]
public class TransferFixtureImportTests
{
    private static readonly string FixturesRoot = Path.Combine(TestContext.CurrentContext.TestDirectory, "Transfer", "Fixtures");

    private static IEnumerable<string> Fixtures() =>
        Directory.EnumerateFiles(FixturesRoot, $"*{TransferFormat.FileExtension}", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(FixturesRoot, path));

    [TestCaseSource(nameof(Fixtures))]
    public async Task EveryFixture_ImportsAsANewDatasetWithEveryRecord(string fixture)
    {
        using SqliteTestFixture database = new();
        DatasetDAO datasetDAO = new(database.RawFactory);

        DatasetSnapshot read;

        await using (FileStream stream = File.OpenRead(Path.Combine(FixturesRoot, fixture)))
        {
            read = TransferDocumentReader.Read(stream).File!.Snapshot;
        }

        DatasetModel imported = await datasetDAO.ImportAsNewAsync("Imported", read);
        DatasetSnapshot saved = await datasetDAO.GetSnapshotAsync(imported.Id);

        (saved.Categories.Count, saved.Components.Count, saved.Blueprints.Count, saved.Favorites.Count)
            .Should().Be((read.Categories.Count, read.Components.Count, read.Blueprints.Count, read.Favorites.Count));
        saved.Blueprints.Sum(blueprint => blueprint.Components.Count + blueprint.Blueprints.Count)
            .Should().Be(read.Blueprints.Sum(blueprint => blueprint.Components.Count + blueprint.Blueprints.Count));
    }
}
