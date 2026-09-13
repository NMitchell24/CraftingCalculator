# Dataset Import/Export — implementation plan

> Source spec: `Z:\Scratch\Dataset-Import-Export.md`. Written 2026-09-12 in a research-and-design session;
> no code has changed yet. One phase per session. Every session updates the checkpoint below.

---

## Status checkpoint — update at the end of every session

| Phase | State | Branch | PR |
|---|---|---|---|
| 1 — Framework (page shell, MaxVisible, drawer collapse) | **built, awaiting commit + PR** | `Import-Export-Phase-1` | — |
| 2 — Data Export | **next** (after Phase 1 merges) | `Import-Export-Phase-2` | — |
| 3 — Data Import | not started | `Import-Export-Phase-3` | — |

**Next session starts at:** Phase 2, step 2.1, on a fresh `Import-Export-Phase-2` branch cut from `main` once
Phase 1's PR has merged. Read **1.6 Phase 1 results** first.

**Session log** (append one line per session: date, phase, what landed, what's left):

- 2026-09-12 — research & design. Plan written. Decisions below settled with Nathan.
- 2026-09-12 — Phase 1. All of 1.1–1.4 landed and verified on the Android phone emulator and Windows (see 1.6).
  Left: Nathan commits through Rider pre-commit and opens the PR; the tablet and iOS rows of 1.5 were not run.

### Rules every session follows

1. Read `CLAUDE.md`, invoke `craftingcalculator-dev`, read this whole plan. The spec is the source for
   *copy and behavior*; this plan is the source for *structure*. Where they disagree, this plan's
   **Settled decisions** win.
2. **Fresh branch per phase**, never reuse a merged one (repo squash-merges):
   `git switch main && git pull && git switch --no-track -c Import-Export-Phase-N`; first push
   `git push -u origin Import-Export-Phase-N`. Phase 1's branch `Import-Export-Phase-1` already exists; use
   it rather than creating it.
3. Before finishing: `dotnet format CraftingCalculator.Tests.slnf --verify-no-changes`, `dotnet test`,
   build warning-free, help pages updated in the same change. Commits go through Rider pre-commit (Nathan
   commits unless the Rider MCP can run those checks).
4. **Update this checkpoint** (table, "next session starts at", session log, tick boxes) as the last
   edit of the session, in the phase's PR.

---

## Context

Users build a dataset by hand — often hundreds of records per game — and today it lives and dies with the
app's storage (`docs/help/settings.md:49-51` says so outright). There is no way to back it up, move it
from a PC to a phone, or share it with a friend. This feature adds per-dataset export to a versioned
`.ccdata` file and a staged, validated import wizard, delivered in three self-contained phases. Phase 1
also pays down two shell debts the fifth Dataset action exposes: `ActionsBar.MaxVisible` is a hard 4
everywhere, and the labeled drawer can't be collapsed.

---

## Settled decisions — do not reopen

| # | Decision | Why |
|---|---|---|
| D1 | The Import/Export action goes on the **Dataset landing page** (`/dataset`, `Dataset.razor.cs:46-51`), inserted after Blueprints, before Delete all data. | It's the dataset-level screen; the spec's "DatasetList" meant this page. |
| D2 | Export file = **`.ccdata`**, UTF-8 JSON with a versioned envelope. | Branded, recognizable in Files apps, associable later. |
| D3 | Export folder: **iOS → `Documents/Exports`** with `UIFileSharingEnabled` + `LSSupportsOpeningDocumentsInPlace` (visible in Files → On My iPhone). **Android/Windows → `FileSystem.AppDataDirectory/Exports`.** Card shows full path on Windows; file name + "Saved in app storage" (Android) / "Files → On My iPhone → Crafting Calculator" (iOS). | AppDataDirectory is invisible on phones; the iOS sandbox path has a GUID that changes on reinstall. |
| D4 | Plan lives at `docs/import-export-plan.md`. | Travels with the branches. |
| D5 | Import/export covers **one dataset** at a time. | Spec note. |
| D6 | Routes: `/dataset/import-export`, `/dataset/import-export/export`, `/dataset/import-export/import`. | Literal segments beat `DatasetList`'s `/dataset/{Type}`; the Dataset nav link stays highlighted; help resolves by prefix. |
| D7 | Defaults the spec left open, decided here: "Latest export" is the newest file in the folder regardless of dataset, and the card names the dataset it came from. Import **As New Dataset** does *not* switch to the new dataset (toast names it). Select-all on Categories asks the same "also select what uses these?" prompt as a single category. A panel header is highlighted when *any* of its records is selected. | Keep the spec's behavior, fill its gaps. Confirm with Nathan at phase start only if something reads wrong in the device run. |
| D8 | **Cyclic blueprint data is invalid everywhere.** An import file with a cycle fails validation. An export of legacy cyclic data is refused with the blueprint names. A merge that would create a cycle is blocked. | The editor already prevents cycles; `BlueprintDAO.BuildModel`'s guard only keeps old data from crashing the app. |
| D9 | **Android minimum API goes from 24 to 35 in Phase 3** (step 3.2). | Nathan's intended release floor; no storage permission is needed for the picker. |
| D10 | App id becomes `com.sterlingtp.craftingcalculator` before release. No action in this feature beyond keeping the `.ccdata` UTI in one constant. | See Risks. |

---

## Cross-phase architecture (built in Phase 2, reused in Phase 3)

### The one shape both screens read: `DatasetSnapshot`

Flat, id-keyed, dumb records — the same shape whether the rows came from the database (export) or from a
validated file (import). Everything downstream (selection, panels, info dialog, file writer, importer)
takes a snapshot, so the export and import screens share all of it.

```
Domain/Enums/RecordKind.cs          Category, Component, Blueprint, Favorite
Domain/Models/Transfer/
  RecordKey.cs                      readonly record struct RecordKey(RecordKind Kind, int Id)
  DatasetSnapshot.cs                string DatasetName; lists of the four records below
  SnapshotCategory.cs               Id, Name, Description
  SnapshotComponent.cs              Id, Name, Description, Cost, ProductionTime, int? CategoryId
  SnapshotBlueprint.cs              Id, Name, Description, Value, Yield, ProductionTime, int? CategoryId,
                                    IReadOnlyList<QuantityLink> Components, IReadOnlyList<QuantityLink> Blueprints
  SnapshotFavorite.cs               Id, Name, IReadOnlyList<QuantityLink> Blueprints
  QuantityLink.cs                   record QuantityLink(int TargetId, long Quantity)
```

`RecordKind` is a new enum rather than a `Favorite` member on `DataType`: `DataType` is the discriminator
for the editable Dataset lists (`/dataset/{Type}` parses it; `RecordService` switches on it), and a
`Favorite` arm would be dead in every one of those switches.

### Dependency engine — the reusable, schema-change-safe core

```
Application/BusinessLogic/Processors/DependencyGraphProcessor.cs
  static DependencyGraph Build(DatasetSnapshot snapshot)          // THE ONLY place that knows the edges
  static IReadOnlyList<string> FindCycles(DependencyGraph graph)   // blueprint names in any cycle
Domain/Models/Transfer/DependencyGraph.cs
  IReadOnlyDictionary<RecordKey, IReadOnlyList<RecordKey>> DependsOn, UsedBy; IReadOnlyList<RecordKey> All
Application/BusinessLogic/Processors/TransferSelectionProcessor.cs
  static SelectionChange Select(DependencyGraph g, IReadOnlySet<RecordKey> selected, RecordKey key)
  static SelectionChange Deselect(...key)
  static SelectionChange SelectAll(...RecordKind kind) / DeselectAll(...kind)
  static SelectionChange SelectUsersOf(...RecordKey category)      // the category "Yes" shortcut
  static SelectionState StateOf(DependencyGraph g, IReadOnlySet<RecordKey> selected, RecordKind kind)  // All/None/Some
  static bool IsClosed(DependencyGraph g, IReadOnlySet<RecordKey> selected)   // every dependency present
Domain/Models/Transfer/SelectionChange.cs
  IReadOnlySet<RecordKey> Added, Removed;  IReadOnlyDictionary<RecordKind,int> CascadedByKind  // excludes the tapped key(s)
```

Edges (`Build`): Component → its Category; Blueprint → its Category, each linked Component, each child
Blueprint; Favorite → each Blueprint. `UsedBy` is the reverse.

Rules, all expressed as graph walks with a visited set. The visited set is for shared sub-recipes (Bronze
reached through both the Axe and the Nails), not for cycles. **Cyclic data is invalid**: the editor already
prevents cycles, and `BlueprintDAO.BuildModel`'s ancestor guard exists only so pre-protection data can't
crash the app. Import validation rejects a cyclic file (3.1). Export runs the same check before writing
and names the offending blueprints rather than producing a file that could never be imported (2.3).

- **Select** = key ∪ transitive `DependsOn` closure (not already selected).
- **Deselect** = key ∪ transitive `UsedBy` closure (currently selected).
- **SelectUsersOf(category)** = direct `UsedBy` of the category, each with its `DependsOn` closure.
- **All-variants** = union over every record of the kind.
- Processors are pure and return a *preview*; the UI decides whether to prompt from
  `CascadedByKind`, then applies `Added`/`Removed` to its `HashSet<RecordKey>`. One prompt builder
  (`UI/Components/Dialogs/TransferPrompts.cs`, same shape as `DatasetPrompts.cs`) maps
  (action, kind, `CascadedByKind`) to the spec's messages, rewritten in app voice:
  - deselect with cascade → Continue/Cancel, text lists what else will be deselected by kind (blueprint
    with *only* favorites cascaded gets the short favorites wording);
  - select a category (or select-all categories) with unselected users → Yes/No "also select…";
  - select anything else, or deselect a favorite → no prompt.

A schema change (e.g. a new link table) touches `DependencyGraphProcessor.Build` and the snapshot types —
nothing in the selection rules or the UI. `DependencyGraphProcessorTests` pins every edge.

### File format — versioned, frozen, always backward compatible

```json
{
  "format": "crafting-calculator-dataset",
  "formatVersion": 1,
  "exportedAt": "2026-09-12T18:04:00Z",
  "appVersion": "1.0",
  "datasetName": "Valheim",
  "categories": [ { "ref": 1, "name": "Tools", "description": "" } ],
  "components": [ { "ref": 1, "name": "Copper", "description": "", "cost": 0, "productionTime": "00:00:00", "category": null } ],
  "blueprints": [ { "ref": 1, "name": "Bronze", "description": "", "value": 0, "yield": 1, "productionTime": "00:00:00",
                    "category": null, "components": [ { "ref": 1, "quantity": 2 } ], "blueprints": [] } ],
  "favorites":  [ { "ref": 1, "name": "Bronze Axe run", "blueprints": [ { "ref": 1, "quantity": 5 } ] } ]
}
```

- `ref`s are **file-local**, renumbered 1..n per type at export — database ids never leave the device.
- DTOs live in `Application/BusinessLogic/Transfer/Format/V1/` (`TransferDocumentV1` etc.) with a
  source-generated `TransferJsonContext : JsonSerializerContext` (trim-safe for Android/iOS Release —
  iOS Release runs the interpreter with a link description, `CraftingCalculator.UI.csproj:44-52`).
  `System.Text.Json` ships with net10.0 — no package.
- **Freeze rule:** once a format version has shipped, its DTOs and its fixture files never change. A
  schema change that affects the file = new `V2` DTOs + an upgrader `V1ToV2 : JsonNode → JsonNode` +
  `CurrentFormatVersion = 2`. The reader peeks `formatVersion`, runs the upgrader chain, then deserializes
  the current DTOs. A file newer than the app is rejected with "made by a newer version of Crafting
  Calculator — update the app".
- **Tripwires (tests) that enforce it:**
  - `TransferFixtureTests` — every file in `tests/.../Transfer/Fixtures/v*/` must validate and import
    forever; fails if `CurrentFormatVersion` has no fixture.
  - `TransferSchemaTripwireTests` (Infrastructure tests) — reads EF model metadata for `Category`,
    `Component`, `Blueprint`, `Favorite` and the three link entities and compares the property list to a
    pinned list. Adding a column fails the test with a message: "decide whether the export format needs a
    new version, then update this list."

---

## Phase 1 — Framework

**Goal:** the Import/Export page exists behind a 5th Dataset action; the ActionsBar knows how many slots
each device really has; the labeled drawer can be collapsed and remembers it.

### 1.1 Import/Export action and page

- [x] `Dataset.razor.cs:46-51` — add
  `new PageAction("Import/Export", Icons.Material.Filled.ImportExport, () => Navigate("/dataset/import-export"))`
  after the `SectionSpecs` actions, before "Delete all data".
- [x] New `UI/Components/Pages/ImportExport.razor` + `.razor.cs` — `@page "/dataset/import-export"`,
  `PageShellConfig("Import/Export Data") { BackHref = "/dataset" }`, `Reset(this)` in `Dispose`
  (pattern: `Settings.razor.cs`).
- [x] Two large square tappable cards (a shared private markup block, not a new control — single use):
  - **Export Data** — `Icons.Material.Filled.Output` — "Export any of your Categories, Components,
    Blueprints, or Favorites to back them up or share with friends."
  - **Import Data** — `Icons.Material.Filled.Input` — "Import Categories, Components, Blueprints, or
    Favorites from a backup file that you created, or one that a friend shared with you."
  - Tap → `Snackbar.Add("Coming Soon!", Severity.Info)` (Phase 2/3 replace with navigation).
  - ≥44px targets; `role="button"`, keyboard-activatable.
- [x] CSS in `wwwroot/app.css` (new `.transfer-hub` block near `.dataset-page`):
  - portrait: one column, two rows; landscape (`@media (orientation: landscape)`): two columns.
  - each card `aspect-ratio: 1`, sized to the smaller of the track's width and height so both fit without
    scrolling: compute available height from `100dvh` minus the existing chrome vars
    (`--app-bar-bottom`, `--bottom-nav-height`, `--actions-bar-height`, safe areas — `app.css:117-133`).
    **Don't use container query units** — iOS minimum is 15.0 (`UI.csproj:30`) and `cqh` needs Safari 16.
    `dvh` is Safari 15.4+; verify on the iOS 15 simulator or fall back to `vh`.
  - `max-width: 360px` per card so a 4K window doesn't produce billboard cards.

### 1.2 MaxVisible per device

Today: `ActionsBar.razor.cs:32` `MaxVisible = 4`, never passed; overflow menu takes one slot
(`:37-45`). Rules to implement, computed in `MainLayout.razor.cs` and passed to both `ActionsBar`
instances (`MainLayout.razor:57,68`):

| Where | Condition | MaxVisible |
|---|---|---|
| Bottom bar | `ShowBottomNav` (Xs) | 4 |
| Drawer, phone | `DeviceInfo.Current.Idiom == DeviceIdiom.Phone` | 4 |
| Drawer, tablet | `Idiom == Tablet` | fit to height: `max(4, floor((windowHeight − appBarBottom − 3 × navLinkHeight − divider) / navLinkHeight))` |
| Drawer, desktop | `Idiom == Desktop` (Windows now, Mac Catalyst later) | `int.MaxValue` — no overflow menu; drawer content scrolls independently |

- [x] `private int ActionsMaxVisible` in `MainLayout.razor.cs`, next to `ShowFullDrawer`. Nav-link and
  divider heights: measure the rendered `MudNavLink` in the Browser pane / WebView devtools and declare
  them as named constants with an inline comment saying where the number came from.
- [x] Desktop scroll: `.mud-drawer .mud-drawer-content { overflow-y: auto; }` (scoped so the bottom-bar
  `ActionsBar` is unaffected); verify the main content does not scroll with it.
- [x] Re-evaluated on every viewport notification (already re-renders via
  `NotifyBrowserViewportChangeAsync`, `MainLayout.razor.cs:172`).
- [x] Update the `MaxVisible` XML doc to state the contract ("slots including overflow; `int.MaxValue`
  means never overflow").

> **Existing smell to surface, not fix silently:** drawer-mode overflow items ignore `Active` and
> `OnLongPress` (`ActionsBar.razor:58-66`). No Dataset-landing action uses either, so Phase 1 is unaffected,
> but on a phone in landscape DatasetList's Delete (long-press to exit) would lose its long-press if a 5th
> action were ever added there. Tell Nathan; default is leave it.

### 1.3 Collapsible labeled drawer

- [x] `MainLayout.razor.cs`: inject `IPreferenceStore`; `private const string DrawerCollapsedKey = "drawer_collapsed";`
  (snake_case to match `ThemeState`'s `theme_mode`). Read once in `OnInitialized`; write on toggle. No new
  state class — MainLayout is its only consumer.
- [x] `private bool DrawerExpanded => ShowFullDrawer && !_drawerCollapsed;` and on `MudDrawer`
  (`MainLayout.razor:52`): `Open="@DrawerExpanded"`, `Variant="@(DrawerExpanded ? Persistent : Mini)"` —
  keep the existing "never `Open=true` on Mini" rule from the comment at `:47-51`.
- [x] App bar `.app-bar-start` (`MainLayout.razor:24-30`): when `ShowFullDrawer`, render first a
  `MudIconButton` with `Icons.Material.Filled.MenuOpen` (expanded) / `Icons.Material.Filled.Menu`
  (collapsed), `aria-label` "Collapse menu"/"Expand menu", then the existing back arrow. Check both fit the
  88px side track (`--app-bar-side-width`, `app.css:129`) — two 44px buttons do exactly; widen the var only
  if they don't.
- [x] No auto-expand/collapse: `OpenMiniOnHover` stays unset. Shrinking below 960×600 shows the Mini rail
  as today regardless of the preference; growing back restores the user's choice.

### 1.4 Help (same change)

- [x] New `docs/help/import-export.md` (front matter `title: Import and Export`, `nav_order` after
  managing-datasets; renumber following pages' `nav_order` only if they collide). Phase 1 content: what the
  screen is for, the two cards, and an honest "both are coming soon" note.
- [x] `HelpTopics.cs`: `new("import-export", "Import and Export", "...", ["dataset/import-export"])` after
  `managing-datasets`.
- [x] `docs/help/actions-bar.md`: how many actions each device shows, the ![More](assets/more-vert.svg)
  overflow menu, desktop scrolling, and the ![Collapse](assets/menu-open.svg) / ![Expand](assets/menu.svg)
  toggle (update the note at `:43-44`).
- [x] `docs/help/dataset.md` + `managing-datasets.md`: the ![Import/Export](assets/import-export.svg)
  action; link to `import-export.md`.
- [x] New icons in `docs/help/assets/` (template per skill): `import-export.svg`, `menu-open.svg`,
  `menu.svg`, `more-vert.svg`, `output.svg`, `input.svg`. Embedded resource on Application only.

### 1.5 Phase 1 verification

- `dotnet test` (HelpServiceTests catch a missing page/h1/bad link), format, build 0 warnings.
- Device matrix via the `run` skill: Android phone portrait (5 actions → 3 + overflow on Dataset landing),
  phone landscape (Mini rail, 3 + overflow), Android tablet emulator (fits more, no overflow at full
  height), Windows desktop (all actions, drawer scrolls when window is short, toggle collapses/expands,
  state survives restart), iOS simulator (cards fit portrait and landscape, iOS 15 `dvh`).
- Tap both cards → "Coming Soon!". **?** on the new page opens `import-export` help.

### 1.6 Phase 1 results (2026-09-12)

**Verified:** format clean, 311 tests pass, Android and Windows heads build with 0 warnings.

- Android phone (Pixel 9 emulator, API 37), portrait: Dataset bar shows Categories, Components, Blueprints +
  More (Import/Export, Delete all data). Hub cards are 360px squares above the bottom nav; both toast "Coming
  Soon!". **?** opens `import-export` and closes back to the hub.
- Same phone, landscape: Mini rail shows 3 + More. Cards are 292px squares side by side, no scrolling.
- Keyboard: with the IME open (viewport 997 → 685px) the list search keeps focus and filters normally.
- Windows: all 5 actions in the drawer, no overflow menu. Collapse/expand works and survives a restart.
  Height-only shrink below 600px hides the toggle and shows the Mini rail; growing restores the choice. A
  short window scrolls the drawer on its own; the page does not move.

**Deviations from the steps above:**

- Cards are native `<button>`s rather than `div role="button"`; keyboard activation comes with the element.
- Measured constants: `NavLinkHeight = 40`, `AppBarHeight = 48`, `ActionsDividerHeight = 17`.
- `MainLayout` sets `ResizeOptions.NotifyOnBreakpointOnly = false`. MudBlazor's default only notifies on
  breakpoint (width) changes, so `ShowFullDrawer`'s height gate never updated on a height-only resize, and
  the tablet fit-to-height would have been stale.
- The toggle uses `Edge="Edge.Start"` so its icon lines up with the drawer icons (Nathan's review). App-bar
  icon buttons are 48px, not 44px; toggle + back arrow come to 84px and fit the 88px track.
- `ActionsBar.MaxVisible` is now `EditorRequired` with no default, since both callers pass it.
- `import-export.md` is `nav_order: 9`; dataset, favorites, calculations, settings and tips moved to 10–14.

**Not run:** the Android tablet emulator (fit-to-height path) and the iOS simulator (`dvh`; iOS < 15.4 falls
back to `vh` through `@supports`).

**Surfaced, not fixed:** drawer overflow items ignore `Active`/`OnLongPress` (`ActionsBar.razor:58-66`). The
right-hand app-bar pair is 96px in the 88px track (pre-existing, absorbed by the title's 8px padding; the
`--app-bar-side-width` comment in `app.css` still says 44px buttons).

---

## Phase 2 — Data Export

**Goal:** choose records with dependency-safe selection, write a `.ccdata` in the background, keep the last
five, show the latest, share it.

### 2.1 Domain + engine (see Cross-phase architecture)

- [ ] `RecordKind`, `RecordKey`, snapshot types, `DependencyGraph`, `SelectionChange`, `SelectionState`.
- [ ] `DependencyGraphProcessor`, `TransferSelectionProcessor` + exhaustive tests
  (`tests/CraftingCalculator.Application.UnitTests/BusinessLogic/Processors/`): every rule in the spec's
  "Selection behaviors" list is one or more tests using the Valheim Bronze chain (Copper + Tin → Bronze →
  Bronze Axe; a "Metals" category; a favorite holding Bronze Axe). Include a shared sub-recipe
  (diamond) case, a category-with-no-users case, and `FindCycles` cases (none, self-reference, two-blueprint
  loop, deep loop).

### 2.2 Loading the snapshot

- [ ] `IDatasetDAO.GetSnapshotAsync(int datasetId)` in `DatasetDAO` — same scoping trick as `CopyAsync`
  (`context.DatasetId = datasetId`), seven `AsNoTracking` queries (4 records + 3 link tables), mapped to
  snapshot records. Do **not** go through `BlueprintFavoritesDAO.GetBlueprintQuantitiesAsync` — it reloads
  the whole blueprint graph per row.
- [ ] `DatasetDAOTests`/new `DatasetSnapshotTests`: every field and link round-trips; another dataset's rows
  never appear.

### 2.3 Writing the file

- [ ] `Application/BusinessLogic/Transfer/TransferDocumentProcessor.cs`:
  `static TransferDocumentV1 ToDocument(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, DateTimeOffset now, string appVersion)`
  — renumbers refs, drops unselected, asserts `IsClosed` (throws `InvalidOperationException` — callers
  only pass closed selections; the service catches, see 2.5).
- [ ] Cycle check before writing: the one cycle finder lives in
  `Application/BusinessLogic/Processors/DependencyGraphProcessor.cs` (`FindCycles(DependencyGraph) → names`)
  and is shared by export, import validation (3.1), and the merge check (3.3). On export, a non-empty result
  becomes the page's error ("Bronze Plate and Bronze Nails contain each other — fix one of them in the
  editor, then export again") and no file is written.
- [ ] `Application/Common/Interfaces/IExportFileStore.cs` + `Infrastructure/Files/ExportFileStore.cs`
  (constructed with the export directory; registered in `Infrastructure/DependencyInjection.cs` — add an
  `exportsPath` parameter to `AddDatabaseServices` or a sibling `AddFileServices(exportsPath)`):
  - `Task<ExportFileInfo> SaveAsync(TransferDocumentV1 doc, string datasetName)` — file name
    `{SafeDatasetName}-{yyyyMMdd-HHmmss}.ccdata` (strip invalid file-name chars); write to `*.tmp` then
    `File.Move` so a killed app never leaves a half file as "latest"; then prune to the newest 5 `.ccdata`.
  - `ExportFileInfo? GetLatest()` — re-scans the folder every call (covers "user deleted the files").
  - `ExportFileInfo` record (Domain/Models/Transfer): `FullPath`, `FileName`, `DatasetName`,
    `CreatedAt` — dataset name read from the envelope only if cheap; otherwise parse it from the file name.
  - Tests with a temp directory: naming, atomic write, prune-to-5, latest after manual delete, empty folder.
- [ ] `MauiProgram.cs`: pick the directory per D3 —
  `#if IOS Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Exports")`
  `#else Path.Combine(FileSystem.AppDataDirectory, "Exports")`. Never persist the absolute path.
- [ ] `Platforms/iOS/Info.plist`: `UIFileSharingEnabled`, `LSSupportsOpeningDocumentsInPlace`,
  `UTExportedTypeDeclarations` for `com.nathanmitchell.craftingcalculator.ccdata` (conforms to
  `public.json`, `public.data`; extension `ccdata`; MIME `application/x-ccdata`), and
  `CFBundleDocumentTypes` (role Editor, rank Owner). Declared in Phase 2 because export creates the type;
  Phase 3's picker depends on it. Keep the UTI string in `TransferFormat.UniformTypeIdentifier` (see Risks).

### 2.4 Sharing

- [ ] `Application/Common/Interfaces/IShareService.cs` + `UI/Platform/ShareService.cs` (pattern:
  `ClipboardService`): `Task ShareFileAsync(string path, string title)` → `Share.Default.RequestAsync(new ShareFileRequest { … })`.
  - **Android:** copy the file into `FileSystem.CacheDirectory/sharing-root/` first and add
    `Platforms/Android/Resources/xml/microsoft_maui_essentials_fileprovider_file_paths.xml` restricting the
    FileProvider to `sharing-root` (MAUI docs: without it the provider can expose the whole cache/app data).
  - **iPad / Mac:** set `PresentationSourceBounds` (iPad popover requirement).
  - No permissions required on Android, iOS, or Windows for sharing a non-media file (verified against MAUI
    docs; iOS photo-library keys only apply to media).

### 2.5 Service + session state

- [ ] `IDatasetTransferService` / `DatasetTransferService` (Application, registered in
  `Application/DependencyInjection.cs`):
  - `Task<DatasetSnapshot> LoadCurrentSnapshotAsync()`
  - `Task<ExportFileInfo> ExportAsync(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected)`
  - `ExportFileInfo? GetLatestExport()`
- [ ] `UI/State/ExportState.cs` (scoped, registered in `MauiProgram.cs:49-51`) — owns only what must
  outlive the page: `bool IsRunning`, `ExportFileInfo? Latest`, `string? LastError`, `event Action? Changed`,
  and `Task StartAsync(snapshot, selected)` which runs `Task.Run(() => service.ExportAsync(...))`, catches
  every exception into `LastError` (**no ErrorBoundary exists** — a throw that reaches the renderer freezes
  the whole app), refreshes `Latest`, raises `Changed`. Page selection is *not* in the state: every visit
  reloads the snapshot and starts all-selected.
- [ ] Subscribers marshal: `State.Changed += OnExportChanged; void OnExportChanged() => _ = InvokeAsync(StateHasChanged);`
  (pattern: `MainLayout.OnThemeChanged`, `MainLayout.razor.cs:104-108`) — `Changed` can fire off the
  dispatcher.

### 2.6 UI

- [ ] `UI/Components/Controls/TransferSelectionPanels.razor(.cs)` — **shared with Phase 3.** Parameters:
  `DatasetSnapshot Snapshot`, `DependencyGraph Graph`, `HashSet<RecordKey> Selected`,
  `EventCallback SelectionChanged`. Renders `MudExpansionPanels MultiExpansion` with four panels
  (Categories/Label, Components/Inventory2, Blueprints/Handyman, Favorites/Star — reuse the icons from
  `Dataset.razor.cs:61-66`), collapsed by default:
  - header: icon, title, count, then the tri-state toggle left of the chevron —
    All → `CheckCircle` `Color.Primary`; None → `CheckCircleOutline` `Color.Secondary`;
    Some → `IndeterminateCheckBox` `Color.Tertiary`. Tap: All → deselect all; None/Some → select all.
    Stop the click from toggling the panel. Header gets `list-row-selected` styling when state ≠ None.
    Verify `MudExpansionPanel` header-slot parameters with the MudBlazor MCP before writing markup.
  - body: rows sorted by name (ordinal-ignore-case), in a scroll container `max-height` = 10 rows, rendered
    with `<Virtualize ItemSize=…>` (10k-component datasets exist — see
    `Z:\Scratch\DatasetList-Memory-Followup.md`). Row = name left, Info icon right (`@onclick:stopPropagation`),
    `list-row-selected` when selected (`app.css:563-566`). Tap row → select/deselect via the processor.
  - All prompts go through `TransferPrompts` (see engine section).
- [ ] InfoDialog for snapshot records (`InfoDialog.razor.cs:17-26` takes `IBaseDataRecord` and only
  handles Blueprint/Component today):
  - Add Category support (description) and a Favorite view (its blueprints × quantity).
  - Feed it from a snapshot via `Application/BusinessLogic/Processors/SnapshotModelProcessor.cs`
    (`ToComponentModel`, `ToBlueprintModel` building the `ComponentMap`/`BlueprintMap` graph,
    `ToCategoryModel`) — so staged import data (no database rows) shows the same dialog.
  - **Smell to surface:** that builder overlaps `BlueprintDAO.BuildModel` (`BlueprintDAO.cs:159-176`).
    Tell Nathan; options are (a) accept two builders over two input shapes, (b) have `BlueprintDAO` build
    through a snapshot. Default (a) unless he opts in.
- [ ] `UI/Components/Pages/Export.razor(.cs)` — `@page "/dataset/import-export/export"`, title "Export
  Data", `BackHref = "/dataset/import-export"`:
  - load snapshot in `Task.Run`, build graph, select all.
  - `TransferSelectionPanels`, then a `Color.Primary` filled **Export Data** button.
  - actions: `PageAction("Export Data", Icons.Material.Filled.Output, …, Disabled: nothing selected || IsRunning)`,
    `PageAction("Share", Icons.Material.Filled.Share, …, Disabled: Latest is null || IsRunning)`.
  - below the button: running → `MudProgressCircular` + "Building your export…" (non-blocking — **no
    overlay**; the user may navigate away); done → card "Latest export: {dataset} — {local datetime}" + path
    line per D3; `LastError` → error alert.
- [ ] `ImportExport.razor.cs`: Export card navigates to the export page.

### 2.7 Help

- [ ] `import-export.md` export section (selection rules in player terms with the Bronze Axe example,
  where the file goes per platform, share, keep-last-five, "uninstalling still deletes files you didn't
  share").
- [ ] `settings.md:49-55` — replace "no built-in export yet" with a pointer to Import/Export.
- [ ] `managing-datasets.md` "Where they're stored" — mention backing up.
- [ ] Icons: `check-circle.svg`, `check-circle-outline.svg`, `indeterminate-check-box.svg`, `share.svg`.
- [ ] `HelpTopics` route prefix already covers `dataset/import-export/export`.

### 2.8 Phase 2 verification

- Unit tests: engine, snapshot, document processor, file store. `TransferFixtureTests`: export the Bronze
  chain once, **check the file into** `Fixtures/v1/bronze-chain.ccdata`, plus `Fixtures/v1/valheim.ccdata`
  exported from `Z:\Scratch\ValheimSeed` on device.
- `TransferSchemaTripwireTests` in place.
- Device: Android emulator with the Valheim seed — deselect Copper → prompt → Bronze, Bronze Axe and the
  favorite deselect; select the favorite → chain reselects; export, navigate to Craft mid-export, return →
  status correct; export 6 times → 5 files; share to Gmail/Drive. Push
  `Z:\Scratch\CopyDatasetPerf\device-very-large.db3` and confirm the page loads, panels scroll smoothly,
  UI stays responsive during export. iOS simulator: file visible in Files app; iPad share popover. Windows:
  path shown, share sheet opens.

---

## Phase 3 — Data Import

**Goal:** pick a file, validate it safely in the background, review/select like export, then import as a
new dataset or merge into the current one with conflict resolution. Survives navigation, not restart.

### 3.1 Reading and validating

- [ ] `Application/BusinessLogic/Transfer/TransferDocumentReader.cs`:
  `static ImportValidationResult Read(Stream stream)` →
  `ImportValidationResult(DatasetSnapshot? Snapshot, IReadOnlyList<string> Errors)`.
  1. Size cap before parsing (e.g. 50 MB — pick from the very-large fixture's size × 10).
  2. `JsonDocument`/`JsonNode` peek with `JsonReaderOptions { MaxDepth = 16, CommentHandling = Disallow, AllowTrailingCommas = false }`;
     check `format` and `formatVersion` (missing → "not a Crafting Calculator file"; newer → "update the
     app").
  3. Upgrader chain → deserialize with `TransferJsonContext`, `JsonUnmappedMemberHandling.Disallow`.
  4. Semantic validation, collecting **all** errors (with record type + name, capped at ~50 listed):
     unique refs per type; every `category`/`components`/`blueprints` ref resolves; names non-empty after
     trim; quantities ≥ 1; yield ≥ 1; cost/value finite (STJ already rejects NaN/∞ by default);
     production time ≥ 0; string length caps generous enough that any dataset the app can create passes
     (the database has no max lengths — measure the editor's limits, don't invent tighter ones); record
     count caps; **no cycles** in blueprint children (`DependencyGraphProcessor.FindCycles` — a cyclic
     file is a validation error naming the blueprints); nesting ≤
     `BlueprintProcessor.MaxBlueprintDepth` (64).
  5. Map to `DatasetSnapshot` with refs as ids.
  - No polymorphic `$type`, no reflection-based deserialization, nothing from the file is ever rendered as
    markup (Blazor text encoding) or used as a path.
- [ ] Tests: one per error class, malicious shapes (deep nesting, huge strings, duplicate refs, dangling
  refs, cycles, wrong version, truncated file), every fixture passes.

### 3.2 Picking the file

- [ ] `IImportFilePicker` (Application interface) + `UI/Platform/ImportFilePicker.cs` wrapping
  `FilePicker.Default.PickAsync` on the UI thread; **immediately copy** the picked stream to
  `CacheDirectory/import/current.ccdata` (iOS picker opens a security-scoped URL; Android returns a
  `content://` URI) and return that path. Validation then runs on the cached copy in `Task.Run`.
- [ ] File types: iOS `TransferFormat.UniformTypeIdentifier` (declared in 2.3); Android — custom
  extensions have no MIME type, so pass `*/*` and rely on validation; Windows `.ccdata`; macOS `ccdata`.
- [ ] **Bump Android `SupportedOSPlatformVersion` from 24 to 35** (`UI.csproj:31`), settled with Nathan.
  On API 33+ the SAF picker needs no storage permission, so no manifest permission is added. Also update
  `docs/dev-environment.md`'s Android section to state the minimum API, and make sure the emulator
  images used for verification are API 35+.
- [ ] Mac Catalyst (future head, no action now): record in `docs/dev-environment.md` that the sandbox needs
  `com.apple.security.app-sandbox` + `com.apple.security.files.user-selected.read-write` in
  `Platforms/MacCatalyst/Entitlements.plist` wired through `CodesignEntitlements`.

### 3.3 Writing imported data

- [ ] `IDatasetDAO.ImportAsNewAsync(string name, DatasetSnapshot snapshot) → DatasetModel` — same
  one-context, one-transaction, navigation-linked shape as `CopyAsync` (`DatasetDAO.cs:62-243`), sourcing
  from the snapshot instead of the database. Explicit `DatasetId` on each record (`StampDataset`,
  `CraftingDataContext.cs:61-68`, already expects this).
- [ ] **Smell to surface:** `CopyAsync` then equals `ImportAsNewAsync(name, await GetSnapshotAsync(source))`.
  Tell Nathan; offer to re-point `CopyAsync` (covered by `DatasetCopyTests`, 8 tests). Default: leave it.
- [ ] Conflicts: `Application/BusinessLogic/Transfer/ImportConflictProcessor.cs`
  `static IReadOnlyList<ImportConflict> Find(DatasetSnapshot incoming, IReadOnlySet<RecordKey> selected, DatasetSnapshot current)`
  — match by kind + name, trimmed, ordinal-ignore-case. If the current dataset already holds duplicate
  names, match the lowest id and note it inline. `ImportConflict(RecordKind Kind, int IncomingId, int ExistingId, string Name)`.
- [ ] `MergePlan` (Domain/Models/Transfer): incoming snapshot, selected keys, and
  `IReadOnlySet<RecordKey> Replace` (incoming keys whose existing twin is overwritten; every other conflict
  is kept). Keep Mine = empty set; Replace Mine = all conflicts; Select Individually = user's picks.
- [ ] `ImportConflictProcessor.BuildMergedGraph(MergePlan plan, DatasetSnapshot current) → DependencyGraph`,
  checked with the shared `DependencyGraphProcessor.FindCycles` (the same finder export and validation use).
  **Answer to the spec's "double-check me":** selection dependencies don't matter for conflict choices —
  every reference resolves by name to exactly one row either way. **But mixing can create a cycle** that
  neither dataset had. Example: mine has *Bronze Nails* using *Bronze Plate*; the file has *Bronze Plate*
  using *Bronze Nails*. Keep mine for *Bronze Nails*, replace *Bronze Plate* → each now contains the other.
  Full Keep Mine and full Replace Mine can't produce one (proof in a test comment); Select Individually can.
  Run the check on the merged graph before writing; block with a message naming the blueprints so the user
  changes a pick.
- [ ] `IDatasetDAO.MergeAsync(int datasetId, MergePlan plan)` — one transaction: map incoming id → target
  id (existing id for conflicts, new entity for the rest); for replaced records overwrite scalar fields and,
  for blueprints/favorites, replace link rows; kept records untouched; new records' links point at mapped
  ids (existing or new).
- [ ] DAO tests (real SQLite fixture): as-new round trip equals the source; Keep Mine links new blueprints
  to existing components; Replace Mine updates values and links and existing parents still point at the
  replaced row; individual mix; transaction rollback on failure; other datasets untouched.

### 3.4 Service + wizard state

- [ ] Extend `IDatasetTransferService`: `ValidateAsync(string cachedPath)`, `ImportAsNewAsync(snapshot, selected, name)`,
  `FindConflictsAsync(snapshot, selected)` (loads the current snapshot), `MergeAsync(MergePlan)`
  (runs `FindCycles` first).
- [ ] `UI/State/ImportState.cs` (scoped): `ImportStep Step` (`SelectFile`, `Validating`, `Invalid`,
  `Review`, `CheckingConflicts`, `ResolveConflicts`, `Importing`) plus the step's data (errors, staged
  snapshot, graph, `HashSet<RecordKey>` selection, conflicts, individual picks), `event Changed`, and one
  method per transition. Every background step: `Task.Run`, catch into an error step, raise `Changed`;
  subscribers marshal with `InvokeAsync`. Selection **lives here** so the user comes back to their picks.
  `Reset()` returns to `SelectFile` and deletes the cached file.
- [ ] After a merge into the current dataset, the Craft batch may hold stale blueprint models. **Check at
  phase start** what `CraftState` does after a blueprint edit in the editor today and do the same; if it
  does nothing, reload the batch's blueprints by id (don't `Clear()` the user's batch).

### 3.5 UI — `UI/Components/Pages/Import.razor(.cs)`, `@page "/dataset/import-export/import"`

- [ ] `SelectFile`: one Primary button "Select Import File".
- [ ] `Validating`: inline spinner + "Validating file…" (non-blocking).
- [ ] `Invalid`: "This file can't be imported" + a card listing errors + "Choose another file".
- [ ] `Review`: `TransferSelectionPanels` (from Phase 2) + Primary **Import Data** button and matching
  `PageAction` (`Icons.Material.Filled.Input`), disabled when nothing is selected. Import Data →
  `ShowMessageBoxAsync` "How would you like to import this data?" → **As New Dataset** /
  **Into '{current name}'**.
  - As New → `DatasetPrompts.PromptForDatasetNameAsync` (reuse; prefill the file's `datasetName`) →
    `Importing` → toast "Imported into new dataset '{name}'" → `Reset()`.
  - Into Current → `CheckingConflicts` spinner "Checking for conflicts…" → no conflicts: import straight
    away; otherwise prompt built only from non-zero kinds ("3 components and 1 blueprint already exist in
    '{current}'…") → **Keep Mine** / **Replace Mine** / **Choose Each** (MudDialog with three buttons — verify
    API with the MudBlazor MCP; `ShowMessageBoxAsync` offers yes/no/cancel which fits if labeled).
- [ ] `ResolveConflicts`: header "Tap the ones you want to replace. Everything you don't pick stays as
  it is."; list grouped by kind, `list-row-selected` rows; an **Import Selected** Primary button docked above
  the nav (sticky bottom container honoring `--bottom-nav-height` / safe areas; no actions bar on this
  step). Cycle-check failure → dialog naming the blueprints, stay on the step.
- [ ] Every finish path: toast "Data Imported" (or the As-New variant), `Reset()`, and if the target was
  the current dataset, refresh per 3.4.
- [ ] `ImportExport.razor.cs`: Import card navigates to the import page.

### 3.6 Help

- [ ] `import-export.md` import section: picking a file (per platform), what "invalid" means with the
  common causes as troubleshooting headings in the reader's words, review/selection (link the export
  section), As New vs Into Current, Keep/Replace/Choose Each with a worked Valheim example, the cycle
  message, cross-platform (export on PC, import on phone).
- [ ] `managing-datasets.md`: importing creates a dataset → link.
- [ ] Icons as needed (e.g. `input.svg` already added in Phase 1).

### 3.7 Phase 3 verification

- Unit/DAO tests above; all fixtures import forever.
- Device: export Valheim on **Windows**, share/copy the file to the Android emulator and iOS simulator,
  import As New on each, compare record counts and a Bronze Axe craft result with the source. Into Current
  with Keep / Replace / Choose Each, including the constructed cycle case. Navigate away at every step and
  return. Pick a `.txt` renamed to `.ccdata`, a truncated file, and a v999 file → clear errors, app stays
  responsive. Very-large fixture: validation and import keep the UI responsive; watch memory with the
  approach used for the Copy Dataset measurements (import is one insert per row too, and so is its speed).

---

## Risks and things to check

- **SQLite blocks the calling thread** — every DAO call from a page goes in `Task.Run`; UI state mutations
  and `Snackbar.Add` stay on the dispatcher.
- **No ErrorBoundary** — background tasks must catch and convert to state; never let an import/export
  exception reach a render.
- **Import speed ≈ copy speed** (~1,400 rows/s on device). Fine under a non-blocking spinner; don't
  optimize without measuring (see `Z:\Scratch\CopyDataset-Perf-And-Busy-Handoff.md` §2).
- **Fast deployment hides changes** on Android — use `EmbedAssembliesIntoApk=true` for device runs.
- **`dvh` on iOS 15** and **nav-link height constants** in Phase 1 need a real measurement, not a guess.
- **App identifier rename before release.** `com.nathanmitchell.craftingcalculator` becomes
  `com.sterlingtp.craftingcalculator`. Nothing to do now, but the `.ccdata` UTI is derived from it: keep the
  UTI string in one C# constant (`TransferFormat.UniformTypeIdentifier`, used by the picker) plus the
  `Info.plist` declaration, so the rename touches exactly those two places. List them in the rename's own
  checklist when it happens. Already-exported files are unaffected, since the extension and envelope don't
  carry the id.
- **MudBlazor APIs** (`MudExpansionPanel` header content, three-button dialogs, `Virtualize` inside a
  panel) — confirm with the MudBlazor MCP before writing markup.

## Definition of done (whole feature)

- A dataset exported on any platform imports on any other, as new or merged, with every rule in the spec.
- Every shipped format version has a fixture that CI imports on every PR; changing an exported entity fails
  a test until someone decides about the format.
- Help pages describe exactly what the screens do; `settings.md` no longer says there's no export.
- This plan's checkpoint table says all three phases are merged.
