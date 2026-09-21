using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The import wizard: the file being imported, the <see cref="ImportStep"/> it has reached, and every choice made
/// along the way. Scoped, so the user can leave the Import page at any step and come back to the same place; nothing
/// survives a restart. Each transition checks the step it starts from and does nothing from any other. Every step
/// that reads or writes runs in the background and never throws: a failure lands in <see cref="Error"/>.
/// </summary>
/// <remarks>
/// <see cref="Changed"/> can be raised off the renderer's dispatcher, so a component subscribes with
/// <c>_ = FireAndForget.RunAsync(() =&gt; InvokeAsync(StateHasChanged), DispatchExceptionAsync)</c> rather
/// than calling <c>StateHasChanged</c> directly.
/// </remarks>
public sealed partial class ImportState(
    IDatasetTransferService transferService,
    IImportFilePicker filePicker,
    ISelectedDatasetState selectedDataset,
    CraftState craftState,
    ISnackbar snackbar,
    ILogger<ImportState> logger)
{
    private string? _filePath;
    private DatasetSnapshot? _chosen;
    private int _targetDatasetId;

    public ImportStep Step
    {
        get;
        private set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            LogImportStep(logger, value);
        }
    } = ImportStep.SelectFile;

    /// <summary>What went wrong with the last step that failed, in words for the user, or null.</summary>
    public string? Error { get; private set; }

    /// <summary>Every reason the chosen file can't be imported, on <see cref="ImportStep.Invalid"/>.</summary>
    public IReadOnlyList<string> ValidationErrors { get; private set; } = [];

    /// <summary>
    /// The chosen file: the records it holds, and when and by which version of the app it was exported. Set from
    /// <see cref="ImportStep.Review"/> on.
    /// </summary>
    public ImportFile? ImportFile { get; private set; }

    /// <summary>The dependency graph of the records in <see cref="ImportFile"/>.</summary>
    public DependencyGraph? Graph { get; private set; }

    /// <summary>The records of <see cref="ImportFile"/> chosen for import, which the selection panels change in place.</summary>
    public HashSet<RecordKey> Selected { get; } = [];

    /// <summary>The name of the dataset the records are going into, from <see cref="ImportStep.CheckingConflicts"/> on.</summary>
    public string TargetDatasetName { get; private set; } = "";

    /// <summary>The chosen records that share a name with a record in the target dataset.</summary>
    public IReadOnlyList<ImportConflict> Conflicts { get; private set; } = [];

    /// <summary>The key of every record in <see cref="Conflicts"/>.</summary>
    public IReadOnlySet<RecordKey> ConflictKeys { get; private set; } = new HashSet<RecordKey>();

    /// <summary>The conflicts, by the incoming record's key, that replace the target dataset's records.</summary>
    public HashSet<RecordKey> Replace { get; } = [];

    /// <summary>The blueprints the last merge would have nested inside themselves, by name; empty otherwise.</summary>
    public IReadOnlyList<string> CycleNames { get; private set; } = [];

    private bool IsRunning =>
        Step is ImportStep.OpeningFile or ImportStep.Validating or ImportStep.CheckingConflicts or ImportStep.Importing;

    /// <summary>Whether the selection can be imported: on <see cref="ImportStep.Review"/>, with a record selected.</summary>
    public bool CanImport => Step == ImportStep.Review && Selected.Count > 0;

    public event Action? Changed;

    /// <summary>
    /// Asks the user for a file and checks it, starting the wizard over with whatever it holds. Backing out of the
    /// picker leaves the wizard where it was.
    /// </summary>
    public async Task PickFileAsync()
    {
        if (Step is not (ImportStep.SelectFile or ImportStep.Invalid or ImportStep.Review))
        {
            return;
        }

        // The picker only returns once the file is copied, which near the size limit takes seconds. Until then every
        // other transition has to refuse: a second pick would copy over the same file, and a finished import would
        // delete it.
        ImportStep previous = Step;
        Step = ImportStep.OpeningFile;
        Changed?.Invoke();

        string? path;

        try
        {
            path = await filePicker.PickAsync();
        }
        catch (Exception exception)
        {
            // Caught rather than left to the page boundary: this says what failed in the app's own words and
            // keeps the wizard on screen, where the user can pick another file.
            LogFilePickerFailed(logger, exception);
            StartOver();
            Error = "I couldn't open that file. Try again, or choose a different one.";
            Changed?.Invoke();
            return;
        }

        if (path is null)
        {
            Step = previous;
            Changed?.Invoke();
            return;
        }

        // The picker has already replaced the previous copy, so only the staged records are cleared.
        Clear();
        _filePath = path;
        Step = ImportStep.Validating;
        Changed?.Invoke();

        try
        {
            (ImportValidationResult result, DependencyGraph? graph) = await Task.Run(() =>
            {
                using FileStream stream = File.OpenRead(path);
                ImportValidationResult read = TransferDocumentReader.Read(stream);

                return (read, read.File is { } importFile ? DependencyGraphProcessor.Build(importFile.Snapshot) : null);
            });

            if (result.File is { } file && graph is not null)
            {
                ImportFile = file;
                Graph = graph;
                Selected.UnionWith(graph.All);
                Step = ImportStep.Review;
            }
            else
            {
                ValidationErrors = result.Errors;
                Step = ImportStep.Invalid;
            }
        }
        catch (Exception exception)
        {
            // Every exception, for the reason given above.
            LogImportFileUnreadable(logger, exception);
            ValidationErrors = ["I couldn't read that file. Try choosing it again."];
            Step = ImportStep.Invalid;
        }

        Changed?.Invoke();
    }

    /// <summary>Imports the selected records into a new dataset named <paramref name="name"/>, then starts over.</summary>
    public async Task ImportAsNewAsync(string name)
    {
        if (Step != ImportStep.Review || ImportFile is not { Snapshot: var snapshot })
        {
            return;
        }

        HashSet<RecordKey> selected = [.. Selected];
        Error = null;
        Step = ImportStep.Importing;
        Changed?.Invoke();

        try
        {
            DatasetModel created = await Task.Run(() =>
                transferService.ImportAsNewAsync(TransferSelectionProcessor.Extract(snapshot, selected), name));

            Finish($"Imported into new dataset '{created.Name}'");
        }
        catch (Exception exception)
        {
            Fail(ImportStep.Review, exception);
        }
    }

    /// <summary>
    /// Compares the selected records with dataset <paramref name="datasetId"/>, named <paramref name="datasetName"/>.
    /// With no conflicts the records are merged straight away; otherwise the wizard waits on
    /// <see cref="ImportStep.ConflictsFound"/>.
    /// </summary>
    public async Task CheckConflictsAsync(int datasetId, string datasetName)
    {
        if (Step != ImportStep.Review || ImportFile is not { Snapshot: var snapshot })
        {
            return;
        }

        HashSet<RecordKey> selected = [.. Selected];
        _targetDatasetId = datasetId;
        TargetDatasetName = datasetName;
        Error = null;
        Step = ImportStep.CheckingConflicts;
        Changed?.Invoke();

        DatasetSnapshot chosen;

        try
        {
            (chosen, Conflicts) = await Task.Run(async () =>
            {
                DatasetSnapshot extracted = TransferSelectionProcessor.Extract(snapshot, selected);
                return (extracted, await transferService.FindConflictsAsync(datasetId, extracted));
            });
        }
        catch (Exception exception)
        {
            Fail(ImportStep.Review, exception);
            return;
        }

        _chosen = chosen;
        ConflictKeys = new HashSet<RecordKey>(Conflicts.Select(conflict => new RecordKey(conflict.Kind, conflict.IncomingId)));

        if (Conflicts.Count == 0)
        {
            await MergeChosenAsync(chosen, ConflictKeys);
            return;
        }

        Step = ImportStep.ConflictsFound;
        Changed?.Invoke();
    }

    /// <summary>Moves from <see cref="ImportStep.ConflictsFound"/> to picking conflicts one at a time, none picked.</summary>
    public void ChooseEach()
    {
        if (Step != ImportStep.ConflictsFound)
        {
            return;
        }

        Replace.Clear();
        Step = ImportStep.ResolveConflicts;
        Changed?.Invoke();
    }

    /// <summary>Picks or unpicks the conflict with the incoming <paramref name="key"/> for replacing.</summary>
    public void ToggleReplace(RecordKey key)
    {
        if (Step != ImportStep.ResolveConflicts)
        {
            return;
        }

        if (!Replace.Remove(key))
        {
            Replace.Add(key);
        }

        Changed?.Invoke();
    }

    /// <summary>Picks every conflict with an incoming key in <paramref name="keys"/> for replacing.</summary>
    public void ReplaceAll(IEnumerable<RecordKey> keys)
    {
        if (Step != ImportStep.ResolveConflicts)
        {
            return;
        }

        Replace.UnionWith(keys);
        Changed?.Invoke();
    }

    /// <summary>Unpicks every conflict with an incoming key in <paramref name="keys"/>, so each keeps its record.</summary>
    public void KeepAll(IEnumerable<RecordKey> keys)
    {
        if (Step != ImportStep.ResolveConflicts)
        {
            return;
        }

        Replace.ExceptWith(keys);
        Changed?.Invoke();
    }

    /// <summary>
    /// Merges the selected records into the target dataset, replacing the records the conflicts in
    /// <paramref name="replace"/> land on and keeping every other, then starts over. A merge that would nest a
    /// blueprint inside itself writes nothing and stops on <see cref="ImportStep.ResolveConflicts"/> with
    /// <see cref="CycleNames"/> set.
    /// </summary>
    public async Task MergeAsync(IReadOnlySet<RecordKey> replace)
    {
        if (Step is not (ImportStep.ConflictsFound or ImportStep.ResolveConflicts) || _chosen is not { } chosen)
        {
            return;
        }

        await MergeChosenAsync(chosen, replace);
    }

    /// <summary>Drops the conflicts and goes back to choosing records.</summary>
    public void BackToReview()
    {
        if (Step is not (ImportStep.ConflictsFound or ImportStep.ResolveConflicts))
        {
            return;
        }

        ClearConflicts();
        Step = ImportStep.Review;
        Changed?.Invoke();
    }

    /// <summary>Starts the wizard over and deletes the app's copy of the file. Does nothing while a step runs.</summary>
    public void Reset()
    {
        if (IsRunning)
        {
            return;
        }

        StartOver();
        Changed?.Invoke();
    }

    private async Task MergeChosenAsync(DatasetSnapshot chosen, IReadOnlySet<RecordKey> replace)
    {
        // Copied before Replace is refilled, since the caller may be passing Replace itself.
        HashSet<RecordKey> picks = [.. replace];
        int datasetId = _targetDatasetId;
        ImportStep returnTo = Conflicts.Count == 0 ? ImportStep.Review : ImportStep.ResolveConflicts;

        // Kept so a refused merge shows the picks it was refused for, whichever way they were made.
        Replace.Clear();
        Replace.UnionWith(picks);
        Error = null;
        CycleNames = [];
        Step = ImportStep.Importing;
        Changed?.Invoke();

        IReadOnlyList<string> cycles;

        try
        {
            cycles = await Task.Run(() => transferService.MergeAsync(datasetId, chosen, picks));
        }
        catch (Exception exception)
        {
            Fail(returnTo, exception);
            return;
        }

        if (cycles.Count > 0)
        {
            CycleNames = cycles;
            Step = ImportStep.ResolveConflicts;
            Changed?.Invoke();
            return;
        }

        // A replaced blueprint keeps its id, so the Craft batch reloads it by id rather than losing the user's batch.
        if (datasetId == selectedDataset.Id)
        {
            await craftState.ReloadBlueprintsAsync();
        }

        Finish("Data Imported");
    }

    private void Finish(string message)
    {
        snackbar.Add(message, Severity.Success);
        StartOver();
        Changed?.Invoke();
    }

    private void Fail(ImportStep returnTo, Exception exception)
    {
        // Every exception, for the reason given in PickFileAsync. Both writes run in one transaction, so nothing was
        // changed.
        LogImportFailed(logger, exception, returnTo);
        ClearConflicts();
        Error = "I couldn't import your data. Nothing was changed.";
        Step = returnTo == ImportStep.ResolveConflicts ? ImportStep.Review : returnTo;
        Changed?.Invoke();
    }

    private void StartOver()
    {
        if (_filePath is { } path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException exception)
            {
                // The cache folder is the platform's to clear; a copy left behind is overwritten by the next pick.
                LogStagedFileNotDeleted(logger, exception);
            }
        }

        Clear();
        Step = ImportStep.SelectFile;
    }

    private void Clear()
    {
        _filePath = null;
        Error = null;
        ValidationErrors = [];
        ImportFile = null;
        Graph = null;
        Selected.Clear();
        ClearConflicts();
    }

    private void ClearConflicts()
    {
        _chosen = null;
        TargetDatasetName = "";
        Conflicts = [];
        ConflictKeys = new HashSet<RecordKey>();
        Replace.Clear();
        CycleNames = [];
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Import step {Step}")]
    private static partial void LogImportStep(ILogger logger, ImportStep step);

    [LoggerMessage(Level = LogLevel.Error, Message = "The import file picker failed; no file was staged")]
    private static partial void LogFilePickerFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "The staged import file could not be read or validated")]
    private static partial void LogImportFileUnreadable(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error,
        Message = "The import failed and was rolled back, returning to step {ReturnTo}")]
    private static partial void LogImportFailed(ILogger logger, Exception exception, ImportStep returnTo);

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "The staged import file could not be deleted from the cache")]
    private static partial void LogStagedFileNotDeleted(ILogger logger, Exception exception);
}
