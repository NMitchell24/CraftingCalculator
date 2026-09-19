# Creating a new datasetting

A **datasetting** is a setting that belongs to one dataset rather than to the whole app. Each dataset has its own
copy, the app follows the selected dataset's copy, and the settings travel with the dataset through a switch, a copy,
and an export and import. The name is a pun on "dataset settings". It's on purpose, so keep it.

Use Yield was the first one (branch `Datasettings-Use-Yield`). That branch laid all the groundwork, so adding another
setting is a checklist of small edits, plus the code that actually uses the setting. This page is that checklist. Use
Costs and Use Values (branch `Economy-Datasettings`) followed it, and added two settings in one pass. Use Craft Time
(branch `Calculate-Craft-Time`) is the first that only hides UI and leaves the math alone. Currency Name (branch
`Datasetting-CustomCurrency`) is the first that isn't a bool: a nullable string that changes how money is written.

App-wide preferences (theme, export history size) are **not** datasettings. They live in `IPreferenceStore` and on
the Settings screen. If a setting should stay the same when the user switches games, it doesn't belong here.

## How the pieces fit

| Piece | Where | What it does |
|---|---|---|
| `Datasettings` | `Domain/Models/Datasettings.cs` | The record every layer passes around. One parameter per setting, each with its default. |
| `Dataset` entity | `Domain/Entities/Dataset.cs` | One column per setting on the `Datasets` table. |
| `ToSettings` / `ApplySettings` | `Infrastructure/DAO/Impl/DatasetDAO.cs` | The only mapping between the row and the record, one method for each direction. |
| `ISelectedDatasetState.Settings` | `Application/Common/Interfaces` | The selected dataset's settings. `DatasetService` publishes them whenever the selection changes. |
| `IDatasetService.UpdateSettingsAsync` | `Application/Common/Services/Impl/DatasetService.cs` | Applies one change to the selected dataset's saved settings, saves them and publishes them. |
| `TransferDatasettings` | `Application/BusinessLogic/Transfer/Format/TransferDocument.cs` | The `datasettings` block in a `.ccdata` file. |
| The Datasettings card | `UI/Components/Pages/Dataset.razor(.cs)` | One `.datasetting-row` per setting, all saving through `UpdateSettingsAsync`. |

The data flows like this:

```
Datasets row ──ToSettings──▶ DatasetModel.Settings ──DatasetService.Select──▶ ISelectedDatasetState.Settings
                                                                                   │
            Craft screen, editors, dialogs, CraftState.Recalculate  ◀── read ──────┘

Dataset.razor toggle ──▶ Dataset.UpdateSettingsAsync ──▶ IDatasetService.UpdateSettingsAsync ──▶ DatasetDAO.SetSettingsAsync
                               └──▶ CraftState.OnDatasettingsChanged (the batch is recalculated)
```

The calculation processors (`BlueprintProcessor.Flatten`, `BlueprintProcessor.BuildNode`,
`BatchProcessor.CalculateTotals`, `IBlueprintService.GetBlueprintNode`) already take a `Datasettings`. A setting that
changes the math only has to read its own member where it applies. No signature changes.

What happens to the settings in each operation:

| Operation | Settings |
|---|---|
| New dataset | `Datasettings.Default` |
| Copy dataset | Copied from the source (through the snapshot) |
| Export | Always written, whatever records are selected |
| Import as a new dataset | The file's; a file without the block, or without a member, gets the defaults |
| Import into an existing dataset (merge) | Ignored; the dataset keeps its own. `ImportConflictProcessor.Merge` passes `current.Settings` |
| Delete all data | Left alone; that action removes records only |

## The checklist

The example below adds a bool, `UseProductionTime`, on by default. Every step is needed for every setting.

### 1. The record

Add the parameter with its default. Existing `new Datasettings(...)` calls keep compiling, and `Default` picks it up.

```csharp
// Domain/Models/Datasettings.cs
/// <param name="UseProductionTime">Whether ... (document what the app does when it is false).</param>
public sealed record Datasettings(bool UseYield = true, bool UseProductionTime = true)
```

### 2. The column and its migration

```csharp
// Domain/Entities/Dataset.cs
/// <summary>Whether ...; see <see cref="Models.Datasettings.UseProductionTime"/>.</summary>
public bool UseProductionTime { get; set; } = true;
```

```bash
# from src/CraftingCalculator.Infrastructure
dotnet ef migrations add Add<SettingName>Datasetting
```

**Edit the scaffolded `defaultValue` by hand** to the setting's default. The scaffold uses the CLR default (`false`),
which would switch the setting off for every dataset that already exists. Don't use `HasDefaultValue(true)` in the
entity configuration instead: EF leaves a bool that equals its CLR default out of the INSERT, so a store default of
`true` would override a dataset saved with the setting off. `20260918213843_AddDatasettings.cs` is the model.

A nullable setting whose default is `null` needs no edit: the scaffolded nullable column already gives every existing
dataset `null` (`20260919203450_AddCurrencyLabelDatasetting.cs`).

Rewrite the migration's `.cs` file with a file-scoped namespace and no BOM (see `AddDatasettings`), because
`dotnet format` fails CI otherwise. Leave the `.Designer.cs` as generated.

### 3. The DAO mapping

Two lines in `DatasetDAO`, one in each direction. Named arguments, because settings of the same type would otherwise
swap silently:

```csharp
private static Datasettings ToSettings(Dataset entity) =>
    new(UseYield: entity.UseYield, UseProductionTime: entity.UseProductionTime);

private static void ApplySettings(Dataset entity, Datasettings settings)
{
    entity.UseYield = settings.UseYield;
    entity.UseProductionTime = settings.UseProductionTime;
}
```

`SetSettingsAsync`, `ImportAsNewAsync`, `GetSnapshotAsync` and `ToModel` all go through these two, so nothing else in
the DAO changes.

### 4. The export file

Follow `docs/transfer-format-maintenance.md`, Procedure A. For a datasetting that means:

```csharp
// Transfer/Format/TransferDocument.cs: added last, defaulting to the setting's default, so an older file still reads
public sealed record TransferDatasettings(bool UseYield = true, bool UseProductionTime = true);
```

```csharp
// TransferDocumentProcessor.ToDocument
new TransferDatasettings(UseYield: extract.Settings.UseYield, UseProductionTime: extract.Settings.UseProductionTime)

// TransferDocumentReader.ToSnapshot
new Datasettings(UseYield: settings.UseYield, UseProductionTime: settings.UseProductionTime)
```

If the value has rules (a range, an enum that has to be defined), add the check to `DocumentChecker.Run` so a bad file
lists it with every other problem.

Bump `TransferFormat.CurrentVersion` **only** if it equals `TransferFormat.LatestShippedVersion`. While the current
version is still in development, the change goes into it and the fixtures are regenerated (step 8).

### 5. The schema tripwire

Add the column to the `Dataset` entry in
`tests/CraftingCalculator.Infrastructure.UnitTests/TransferSchemaTripwireTests.cs`:

```csharp
(typeof(Dataset), ["Id", "Name", "UseYield", "UseProductionTime"]),
```

### 6. The control on the Dataset screen

One more `.datasetting-row` in the Datasettings card, under the last one of its tab. The card groups the settings into
tabs (`DatasettingsView` in `Dataset.razor.cs`): **General** holds Use Yield and Use Craft Time, **Economy** holds
Use Costs, Use Values and Currency Name. Put the row in the tab it belongs to. A setting that fits neither gets a new
`DatasettingsView` member, a `MudToggleItem` and its own branch in the markup; with more than three tabs, recheck the
`.pane-tabs` container query in `app.css`, which is calibrated for three labels.

The CSS already draws the hairline between rows. The control saves through the page's one `UpdateSettingsAsync`, which
also recalculates the Craft screen's batch, so a row needs no handler of its own. Pass it the **change**, not a finished
`Datasettings`: the service applies updates one at a time to the settings the last one saved, so two switches tapped
while a save is still running both stick. A row that built the whole record from `SelectedDataset.Settings` would
start from the settings before the first tap and undo it:

```razor
<div class="datasetting-row">
    <div class="datasetting-text">
        <MudText Typo="Typo.body1">Calculate Production Time</MudText>
        <MudText Typo="Typo.caption" Class="datasetting-caption">One sentence on what it does.</MudText>
    </div>
    <MudSwitch T="bool" Value="SelectedDataset.Settings.UseProductionTime"
               ValueChanged="@(value => UpdateSettingsAsync(settings => settings with { UseProductionTime = value }))"
               Color="Color.Primary" AriaLabel="Calculate Production Time" />
</div>
```

The label the player reads starts with **Calculate**, not **Use**: `UseYield` is labeled Calculate Yield. The property
keeps the shorter name; only the `MudText` and the `AriaLabel` say Calculate.

A setting that isn't a bool takes whichever control fits (a `MudSelect` for an enum, a `MudNumericField` for a
number) in the same row, bound the same way. A control too wide to sit beside the text, like Currency Name's
`MudTextField`, goes in a `datasetting-row datasetting-row-stacked` row, which puts it full width under the text.
Normalize free text in one place before it's saved (Currency Name's is `CurrencyProcessor.ToLabel`), and run an
imported file's value through the same method in `TransferDocumentReader.ToSnapshot`.

### 7. The feature itself

This part depends on the setting. The patterns Use Yield set:

- **Hiding UI:** inject `ISelectedDatasetState` and wrap the markup in `@if (SelectedDataset.Settings.X)`. Hide the
  field; don't clear the stored value. Turning the setting back on should bring the user's numbers back.
- **Changing the math:** read `settings.X` inside the processor that owns the calculation (Use Yield's is
  `BlueprintProcessor.YieldOf`). `CraftState.Recalculate` already passes the selected dataset's settings in.
- **A tab or pane that goes away:** remove both the toggle item and its pane (see `Craft.razor`), so no hidden view
  can stay selected.

Search the whole UI for every place the feature shows up. Use Yield touched the blueprint editor, the Craft screen's
tabs, the Crafting Summary and `InfoDialog`; Use Costs and Use Values touched the editors, the Crafting Summary,
`InfoDialog` and the `DetailsFor` lines of `MaterialsList` and `SurplusList`. It's easy to miss one.

### 8. Regenerate both v1 fixtures

Both files under `tests/CraftingCalculator.Application.UnitTests/BusinessLogic/Transfer/Fixtures/v{CurrentVersion}/`
have to be regenerated whenever a datasetting is added. The tests **don't** catch a stale `valheim.ccdata`: they
only check that it imports, and an older file still imports because every new member has a default. Regenerate it
anyway. Once the version ships, the fixture is frozen as the example of a real file of that version, and it should
look like one.

**`bronze-chain.ccdata`** is the writer's own output. `TransferFixtureTests` fails until it's regenerated:

```bash
dotnet test tests/CraftingCalculator.Application.UnitTests --filter FullyQualifiedName~WriteTheCurrentBronzeChainFixture
```

The diff should be the new member in the `datasettings` block and nothing else.

**`valheim.ccdata`** is a real export of the Valheim test dataset from a running app:

1. Build and install the branch with fast deployment off (the commands are in the `craftingcalculator-dev` skill,
   "Large text sizes"). A stale install writes the old format.
2. In the app, select the **Valheim** dataset, then **Dataset → Import/Export → Export Data → Export data** with
   everything selected.
3. Pull the newest file out of the app's export folder:

   ```bash
   MSYS_NO_PATHCONV=1 adb exec-out run-as com.sterlingtp.craftingcalculator ls files/Exports
   MSYS_NO_PATHCONV=1 adb exec-out run-as com.sterlingtp.craftingcalculator cat files/Exports/Valheim-<stamp>.ccdata > valheim.ccdata
   ```

4. Before replacing the fixture, compare the two. Apart from `exportedAt` and the `datasettings` block, the records
   should be the same. Refs are numbered in database id order, so if the device's rows were ever re-created the
   records can come out in a different order with different refs. That's fine, as long as a comparison by name (not
   by ref) shows every category, component, blueprint and favorite unchanged. If the data itself differs, the device's
   Valheim dataset has drifted: fix it or reseed it, and don't check in the difference.
5. Copy it over `Fixtures/v{CurrentVersion}/valheim.ccdata` and run `dotnet test CraftingCalculator.Tests.slnf`.

Never touch a fixture under `Fixtures/v{n}` where `n <= LatestShippedVersion`. Those are frozen.

### 9. Tests

Write the tests alongside the feature, not after it:

| What | Where |
|---|---|
| The DAO round-trips the setting and it defaults correctly on a new dataset | `DAO/Impl/DatasetDAOTests.cs` |
| The migration gives existing datasets the right value | `MigrationTests.cs` (migrate to the previous migration, insert a raw row, migrate up) |
| Copy and import-as-new carry it; merge doesn't | `DatasetCopyTests.cs`, `DatasetImportTests.cs` |
| The writer writes it; the reader reads it, and defaults it when the member is missing | `TransferDocumentProcessorTests.cs`, `TransferDocumentReaderTests.cs` |
| The feature's own behavior (the math, with the setting on and off) | The processor's tests, with a `new Datasettings(X: false)` |

`DatasetServiceTests` already covers publishing the whole record on startup, switch, delete and update, and updates
that overlap, so a new setting doesn't need its own tests there.

### 10. Help

User-facing changes update `docs/help/` in the same commit (see CLAUDE.md, "Help content is part of the feature"):

- a `### <Calculate label>` section under `## Datasettings` in `docs/help/dataset.md`, named as the switch is
  (Calculate Yield, not Use Yield): what it does, what it hides, what it changes in the math, and a worked example
  with real numbers
- a short note, linked to that section, on every page that describes something the setting hides or changes

`HelpServiceTests` checks that every cross-page link and anchor resolves.

### 11. Check it on a device

With `font_scale` 2.0 and `wm density` 672 on the emulator (the `craftingcalculator-dev` skill has the commands and
the rules), check that the new row wraps between words, the control stays on the row, and nothing runs off the right
edge. Then flip the setting with a real tap and check that every place the feature touches follows it, that it
survives an app restart, and that switching to another dataset shows that dataset's own value.
