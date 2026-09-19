using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class DatasetServiceTests
{
    private const string SelectedDatasetKey = "selectedDatasetId";

    private static readonly Datasettings NoYield = new(UseYield: false);

    private Mock<IDatasetDAO> _dao = null!;
    private FakePreferenceStore _preferences = null!;
    private SelectedDatasetState _selected = null!;
    private DatasetService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _dao = new Mock<IDatasetDAO>();
        _preferences = new FakePreferenceStore();
    }

    [TearDown]
    public void TearDown() => _service.Dispose();

    /// <summary>
    /// Builds the service over a selection state that has already read <paramref name="storedId"/> back
    /// from the preference store, which is the state a fresh launch starts in.
    /// </summary>
    private void GivenStoredSelection(int? storedId)
    {
        if (storedId is { } id)
        {
            _preferences.Set(SelectedDatasetKey, id.ToString());
        }

        _selected = new SelectedDatasetState(_preferences);
        _service = new DatasetService(_dao.Object, _selected);
    }

    private void GivenDatasets(params DatasetModel[] datasets)
    {
        _dao.Setup(dao => dao.GetAllAsync()).ReturnsAsync([.. datasets]);
        _dao.Setup(dao => dao.CountAsync()).ReturnsAsync(datasets.Length);

        foreach (DatasetModel dataset in datasets)
        {
            _dao.Setup(dao => dao.GetByIdAsync(dataset.Id)).ReturnsAsync(dataset);
        }
    }

    [Test]
    public async Task InitializeAsync_StoredDatasetStillExists_KeepsIt()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(7);
    }

    [Test]
    public async Task InitializeAsync_NothingStored_SelectsTheFirstDatasetAndPersistsIt()
    {
        GivenDatasets(new DatasetModel { Id = 3, Name = "Ark" }, new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(null);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(3);

        // Written back, so the fallback is resolved once rather than on every launch.
        _preferences.Get(SelectedDatasetKey).Should().Be("3");
    }

    [Test]
    public async Task InitializeAsync_StoredDatasetIsGone_FallsBackToTheFirstDataset()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(99);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task InitializeAsync_NoDatasetsAtAll_CreatesDefault()
    {
        GivenDatasets();
        _dao.Setup(dao => dao.AddAsync("Default")).ReturnsAsync(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(null);

        await _service.InitializeAsync();

        _selected.Id.Should().Be(1);
        _dao.Verify(dao => dao.AddAsync("Default"), Times.Once);
    }

    [Test]
    public async Task SwitchToAsync_PersistsTheNewSelection()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);

        await _service.SwitchToAsync(7);

        _selected.Id.Should().Be(7);
        _preferences.Get(SelectedDatasetKey).Should().Be("7");
    }

    [Test]
    public async Task SwitchToAsync_DatasetNoLongerExists_LeavesTheSelectionAlone()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(1);

        await _service.SwitchToAsync(99);

        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task DeleteAsync_TheLastDataset_DeletesNothing()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" });
        GivenStoredSelection(1);

        await _service.DeleteAsync(1);

        _dao.Verify(dao => dao.DeleteAsync(It.IsAny<int>()), Times.Never);
        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task DeleteAsync_TheSelectedDataset_MovesTheSelectionFirst()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);

        await _service.DeleteAsync(7);

        // Moved before the delete: the cascade runs against a dataset nothing is scoped to any more.
        _selected.Id.Should().Be(1);
        _preferences.Get(SelectedDatasetKey).Should().Be("1");
        _dao.Verify(dao => dao.DeleteAsync(7), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ADatasetThatIsNotSelected_LeavesTheSelectionAlone()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);

        await _service.DeleteAsync(7);

        _selected.Id.Should().Be(1);
        _dao.Verify(dao => dao.DeleteAsync(7), Times.Once);
    }

    [Test]
    public async Task NameExistsAsync_TrimsBeforeComparing()
    {
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.GetByNameAsync("Rust", 0)).ReturnsAsync(new DatasetModel { Id = 7, Name = "Rust" });

        (await _service.NameExistsAsync("  Rust  ")).Should().BeTrue();
    }

    [Test]
    public async Task CreateAndRename_TrimTheName()
    {
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.AddAsync("Rust")).ReturnsAsync(new DatasetModel { Id = 7, Name = "Rust" });

        await _service.CreateAsync("  Rust  ");
        await _service.RenameAsync(7, "  Valheim  ");

        _dao.Verify(dao => dao.AddAsync("Rust"), Times.Once);
        _dao.Verify(dao => dao.RenameAsync(7, "Valheim"), Times.Once);
    }

    [Test]
    public async Task CopyAsync_TrimsTheNameAndCopiesTheDatasetItIsGiven()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);
        _dao.Setup(dao => dao.CopyAsync(7, "Rust - Modded")).ReturnsAsync(new DatasetModel { Id = 9, Name = "Rust - Modded" });

        DatasetModel copy = await _service.CopyAsync(7, "  Rust - Modded  ");

        copy.Id.Should().Be(9);

        // The selection is the caller's to move: copying a dataset other than the selected one is a
        // supported call, and it does not make that one the one you are working in.
        _selected.Id.Should().Be(1);
    }

    [Test]
    public async Task InitializeAsync_StoredDatasetStillExists_LoadsItsSettings()
    {
        GivenDatasets(new DatasetModel { Id = 7, Name = "Rust", Settings = NoYield });
        GivenStoredSelection(7);

        await _service.InitializeAsync();

        _selected.Settings.Should().Be(NoYield);
    }

    [Test]
    public async Task SwitchToAsync_PublishesTheNewDatasetsSettings()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust", Settings = NoYield });
        GivenStoredSelection(1);
        await _service.InitializeAsync();

        await _service.SwitchToAsync(7);
        _selected.Settings.Should().Be(NoYield);

        await _service.SwitchToAsync(1);
        _selected.Settings.Should().Be(Datasettings.Default);
    }

    [Test]
    public async Task DeleteAsync_TheSelectedDataset_PublishesTheReplacementsSettings()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default", Settings = NoYield }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);
        await _service.InitializeAsync();

        await _service.DeleteAsync(7);

        _selected.Settings.Should().Be(NoYield);
    }

    [Test]
    public async Task UpdateSettingsAsync_SavesTheChangeToTheSelectedDatasetAndPublishesIt()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Default" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(7);
        await _service.InitializeAsync();

        await _service.UpdateSettingsAsync(settings => settings with { UseYield = false });

        _dao.Verify(dao => dao.SetSettingsAsync(7, NoYield), Times.Once);
        (_selected.Id, _selected.Settings).Should().Be((7, NoYield));
    }

    [Test]
    public async Task UpdateSettingsAsync_ASwitchDuringTheWrite_LeavesTheNewDatasetsSettingsAlone()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Valheim" }, new DatasetModel { Id = 7, Name = "Rust" });
        GivenStoredSelection(1);
        await _service.InitializeAsync();
        TaskCompletionSource write = new();
        _dao.Setup(dao => dao.SetSettingsAsync(1, NoYield)).Returns(write.Task);

        Task update = _service.UpdateSettingsAsync(settings => settings with { UseYield = false });
        await _service.SwitchToAsync(7);
        write.SetResult();
        await update;

        _dao.Verify(dao => dao.SetSettingsAsync(1, NoYield), Times.Once);
        (_selected.Id, _selected.Settings).Should().Be((7, Datasettings.Default));
    }

    [Test]
    public async Task UpdateSettingsAsync_ASecondUpdateDuringTheFirstWrite_KeepsBothChanges()
    {
        // The row only takes a write once it completes, so an update that reads it before then sees the old settings.
        Datasettings stored = Datasettings.Default;
        TaskCompletionSource firstWrite = new();
        _dao.Setup(dao => dao.GetByIdAsync(1)).ReturnsAsync(() => new DatasetModel { Id = 1, Name = "Valheim", Settings = stored });
        _dao.Setup(dao => dao.SetSettingsAsync(1, It.IsAny<Datasettings>()))
            .Returns(async (int _, Datasettings settings) =>
            {
                await firstWrite.Task;
                stored = settings;
            });
        GivenStoredSelection(1);
        await _service.InitializeAsync();

        Task first = _service.UpdateSettingsAsync(settings => settings with { UseCosts = false });
        Task second = _service.UpdateSettingsAsync(settings => settings with { UseValues = false });
        firstWrite.SetResult();
        await Task.WhenAll(first, second);

        Datasettings both = new(UseCosts: false, UseValues: false);
        stored.Should().Be(both);
        _selected.Settings.Should().Be(both);
    }

    [Test]
    public async Task UpdateSettingsAsync_TheDatasetIsGone_SavesNothing()
    {
        GivenDatasets(new DatasetModel { Id = 1, Name = "Valheim" });
        GivenStoredSelection(1);
        await _service.InitializeAsync();
        _dao.Setup(dao => dao.GetByIdAsync(1)).ReturnsAsync((DatasetModel?)null);

        await _service.UpdateSettingsAsync(settings => settings with { UseYield = false });

        _dao.Verify(dao => dao.SetSettingsAsync(It.IsAny<int>(), It.IsAny<Datasettings>()), Times.Never);
        _selected.Settings.Should().Be(Datasettings.Default);
    }
}
