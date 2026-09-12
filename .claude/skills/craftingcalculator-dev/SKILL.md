---
name: craftingcalculator-dev
description: Architecture, conventions, and workflows for the CraftingCalculator .NET MAUI Blazor Hybrid app. Use whenever working on this codebase — adding/editing features, services, DAOs, entities, Blazor pages/components, EF Core migrations, seed data, or tests in the CraftingCalculator.Domain / CraftingCalculator.Application / CraftingCalculator.Infrastructure / CraftingCalculator.UI projects.
---

# CraftingCalculator — Developer Skill

Helper app for survival crafting games: users maintain ingredients and recipes (recipes can nest
other recipes), pick recipes with quantities, and see total raw materials, cost, value, and profit.
**.NET 10 MAUI Blazor Hybrid** app using **MudBlazor** for UI and **EF Core + SQLite** for local
storage. Organized as a Clean-Architecture-style solution.

> **Running/launching the app (Rider/VS), plus Android & Apple/iOS device setup:** see
> [`docs/dev-environment.md`](../../../docs/dev-environment.md) — covers the JDK/Android SDK gotchas,
> the iOS signing Debug/Release split, and building on the Mac.

## Solution layout (`CraftingCalculator.sln`)

Four source projects under `src/` + two mirror test projects under `tests/`.

| Project | TFM | Role |
|---|---|---|
| `CraftingCalculator.Domain` | net10.0 | Entities, Enums, Models, Constants. No deps except EF Core. The core; depends on nothing internal. |
| `CraftingCalculator.Application` | net10.0 | Business logic, services, **all interfaces** (Service + DAO), Processors. References Domain. |
| `CraftingCalculator.Infrastructure` | net10.0 | EF Core `DbContext`, DAO implementations, migrations, SQL seed data. References Application. |
| `CraftingCalculator.UI` | net10.0-android/ios/windows | MAUI Blazor host: Razor pages/components (MudBlazor), DI wiring, platform code. References Infrastructure. |

**Dependency direction:** Domain ← Application ← Infrastructure ← UI. Interfaces (both `IXxxService`
and `IXxxDAO`) live in `Application/Common/Interfaces`; DAO implementations live in
`Infrastructure/DAO/Impl`. Don't put DAO interfaces in Infrastructure.

**Adding a project to `CraftingCalculator.sln`:** `dotnet sln add` stamps the **legacy** C#
project-type GUID `{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}`; after adding, change it in
`CraftingCalculator.sln` to the **SDK-style** `{9A19103F-16F7-4668-BE54-9A1E7A4F7556}` that every
other project uses (the *type* GUID — the first one on the `Project(...)` line — not the project's own
trailing GUID). Mixed GUIDs make VS/Rider treat the project oddly and rewrite the .sln.

## Key conventions & patterns

- **Layered call flow:** Razor page (`@inject IXxxService`) → Service (`Application/Common/Services/Impl`)
  → DAO (`Infrastructure/DAO/Impl`) → `CraftingDataContext`. Services orchestrate; DAOs do data access
  only; pure transformation logic goes in `Application/BusinessLogic/Processors` (e.g.
  `ComponentProcessor`, static methods).
- **DI registration is manual** and split:
  - Services: `src/CraftingCalculator.Application/DependencyInjection.cs` → `AddApplicationServices()`
    (all `AddScoped`).
  - DAOs + DbContext: `src/CraftingCalculator.Infrastructure/DependencyInjection.cs` →
    `AddDatabaseServices(dbPath)`.
  - Both are called from `src/CraftingCalculator.UI/MauiProgram.cs`. **When you add a service or DAO,
    register it in the matching file.**
- **DbContext access:** DAOs inject `IDbContextFactory<CraftingDataContext>` and use
  `await using var context = await contextFactory.CreateDbContextAsync();` per operation (short-lived
  contexts — MAUI/Blazor pattern), primary-constructor style
  (`class BlueprintDAO(IDbContextFactory<CraftingDataContext> contextFactory)`).
  **Never register the `DbContext` itself** — only `AddPooledDbContextFactory<CraftingDataContext>`.
  `BlazorWebView` creates exactly one `IServiceScope` for the WebView's whole lifetime, so a `Scoped`
  `DbContext` would live for the entire session (unbounded change tracker, stale first-level cache,
  `InvalidOperationException` on overlapping async handlers).
- **Naming:** interfaces `IXxxService` / `IXxxDAO`; impls in `Impl/` folders. Models in
  `Domain/Models`, entities in `Domain/Entities`, magic strings/enums in `Domain/Constants` and
  `Domain/Enums` (e.g. `CategoryModel.All`, the currency format string).
- **Domain models vs. EF entities share names** (`Blueprint`, `Component`, `Category`) but live in
  different namespaces — `CraftingCalculator.Domain.Entities` vs. `CraftingCalculator.Domain.Models`.
  Alias at the few call sites (DAO impls) that need both in one file.
- **Read queries use `AsNoTracking()`.** Tracking is confined to the save path inside one
  factory-created context.
- **State a destination page needs rides in the route** (`/dataset/{type}/{id}`), not a shared mutable
  holder. The one exception is the working batch on the Craft screen, which is genuine cross-page
  session state (`CraftState`, scoped, `UI/State`) — components subscribe to its `Changed` event
  in `OnInitialized` and unsubscribe in `Dispose`.
- C# style: `Nullable` and `ImplicitUsings` enabled everywhere; file-scoped namespaces; collection
  expressions (`[]`, `[.. x]`). Match the file you're editing.

## Database, migrations & seed data

- SQLite DB in `FileSystem.AppDataDirectory`. `MauiProgram.CreateMauiApp()` runs
  `db.Database.Migrate()` on startup.
- Migrations in `src/CraftingCalculator.Infrastructure/Migrations/`. Entity config via one
  `IEntityTypeConfiguration<T>` per entity under `Infrastructure/Data/Configurations`, applied with
  `ApplyConfigurationsFromAssembly` in `CraftingDataContext.OnModelCreating`.
- **Seed data is SQL**, not C#: `src/CraftingCalculator.Infrastructure/Data/Seed/*.sql`, embedded as
  resources (`<EmbeddedResource Include="Data\**\*.sql" />`) and executed once by the `InsertSeedData`
  migration via `migrationBuilder.Sql(...)`. The seed is intentionally minimal — just the one category
  row `(Id 1, Name 'All')` that the category dropdown and `CategoryModel.All` depend on. The `.sql` still
  inserts into `RecipeFilters`, the table's pre-rename name, because it runs as applied history —
  `RenameToCraftingVocabulary` renames that table to `Categories` afterwards. **Once released,
  the seed is final — never edit the `.sql` or re-run the seed.** Any data change now ships as a **new
  migration** that Inserts/Updates/Deletes rows on top of the seeded baseline (and never edit an
  already-applied migration).
- **Creating a migration** (run from `src/CraftingCalculator.Infrastructure`):
  ```
  dotnet ef migrations add <Name>
  ```
  No `--startup-project` is needed: `CraftingDataContextFactory` (an `IDesignTimeDbContextFactory`
  using an in-memory SQLite DB) lets EF build the context from this project alone. Tools are
  referenced (`Microsoft.EntityFrameworkCore.Design`/`.Tools`). The DB auto-migrates at app launch; no
  manual `database update` needed for the app.
- **Renaming a table or column: never ship the scaffold as generated.** The model differ matches
  entities by *table name*, so a rename reads to it as one table dropped and an unrelated one
  created — `migrations add` emits `DropTable`+`CreateTable` and prints "An operation was scaffolded
  that may result in the loss of data", which on a real device empties the table. Replace the
  `Up`/`Down` bodies with `RenameTable`/`RenameColumn`/`RenameIndex` (all supported on SQLite; the
  provider rewrites `RenameIndex` as a drop/create of the index, not of the data) and keep the
  generated `.Designer.cs` and snapshot, which describe the resulting model either way. See
  `20260907184851_RenameToCraftingVocabulary`, and
  `MigrationTests.Migrate_FromThePreRenameSchema_PreservesExistingData` for how to cover it: migrate
  to the previous migration by name, insert through the *old* names in raw SQL, migrate up, and read
  the rows back through the new model.
- **Delete behavior is configured explicitly** (cascade / SetNull) per relationship in the entity
  configurations — see the delete-behavior table in the migration plan history / PR description for
  the full list. `Microsoft.Data.Sqlite` enables `PRAGMA foreign_keys` per connection so DB-side
  cascade actually fires; don't reintroduce hand-written cleanup loops in services.

## UI (Blazor + MudBlazor)

- Pages in `src/CraftingCalculator.UI/Components/Pages` (`@page "/..."`), reusable controls in
  `Components/Controls`, dialogs in `Components/Dialogs`, layout in `Components/Layout`.
- **Code-behind pattern:** `Foo.razor` (markup) + `Foo.razor.cs` (`public partial class Foo`). Put
  logic in the `.razor.cs`. Inject services with `@inject IXxxService _name`.
- All UI is **MudBlazor** components; registered via `AddMudServices()`. Global usings in
  `Components/_Imports.razor`.
- **Mobile-first.** Phone portrait is the design target; desktop is the widened case reached by
  breakpoints. Every interactive target is ≥ 44×44 px; no action is reachable only by hover or
  right-click. See the three-destination bottom-nav/side-rail shell (`Craft` `/`, `Favorites`
  `/favorites`, `Dataset` `/dataset`) before adding new navigation.

## Built-in help (`docs/help`)

The app ships a manual. The **?** in the app bar (`MainLayout.ToggleHelp`) opens the help page for the
route the user is on; `/help` is the contents list and `/help/{TopicId}` one page.

**Any change to what the user sees updates the matching page under `docs/help/`, in the same commit.**
Screens, controls, fields, actions, defaults, calculations, user-facing copy — all of it. This is not a
nice-to-have: a stale help page is worse than none, because the user believes it.

**One source, three consumers.** `docs/help/*.md` is the only copy of this content:

| Consumer | Mechanism |
|---|---|
| App | `CraftingCalculator.Application.csproj` embeds `..\..\docs\help\*.md` (`LogicalName` `CraftingCalculator.Application.Help.<file>`); `HelpService` reads the resource and `HelpProcessor.Render` turns it into an HTML fragment with Markdig; `UI/Components/Pages/Help.razor` renders it as a `MarkupString` inside `.help-article` (app.css). |
| GitHub wiki | `.github/workflows/help-sync.yml` on merge to `main` — strips front matter, drops `.md` from links, publishes `welcome.md` as `Home.md` too. |
| Project site | Files already live under `docs/` with Jekyll front matter. Not wired up; see `docs/pages-rewrite-plan.md`. |

That is why the content is Markdown and not Razor, and why it has to stay portable:

- **Plain Markdown only** — no raw HTML, no Liquid, no MudBlazor components.
- **Cross-page links are `other-page.md`** (`calculations.md#surplus` for an anchor). GitHub and the
  wiki resolve those natively; `HelpProcessor.RewriteLink` turns them into `/help/other-page`. Enabled
  Markdig extensions: YAML front matter, pipe tables, auto identifiers, emphasis extras.
- **No external links** — the `BlazorWebView` has nowhere to send them.
- **Front matter on every page**: `title`, `nav_order`.

**Icons.** A control the help names is written as an image: `![Delete](assets/delete.svg)`. The files in
`docs/help/assets/` are the Material Design icons MudBlazor itself draws — each is
`<svg viewBox="0 0 24 24" fill="#888888">` wrapping the path data from the matching
`Icons.Material.Filled.<Name>` constant. To add one, reference MudBlazor from a throwaway console
project and print the constant, then wrap it in that same template; the gray is deliberate, since on
GitHub the file renders as an `<img>` with no text color to inherit. `HelpProcessor.ReplaceIcon` swaps
that fill for `currentColor` and inlines the markup, which is what makes the icon follow the app's
palette; `.help-article .help-icon` (app.css) sizes it in `em` against the text it sits in. Icons are
the only images the help may use, and an image with no matching file degrades to its alt text.

**Never make these MauiImage.** They are `EmbeddedResource` on `CraftingCalculator.Application` on
purpose: `MauiImage` runs every file through the resizetizer, which rasterizes each SVG once per Android
density bucket and per iOS scale — twenty-odd icons would become a few hundred PNGs in the app package,
for images the help renders as inline markup and never loads as a file. All 21 currently cost about 8 KB
inside the assembly. Verify after a change with
`unzip -l <apk> | grep -iE "handyman|unfold|delete_forever"` — it should find nothing.

**Adding a help page:** write `docs/help/<slug>.md` **and** add a `HelpTopic` to
`Domain/Constants/HelpTopics.cs` (`Id` = the file's slug). The catalog drives the contents list and the
route mapping; a file with no entry is embedded but unreachable.

**Adding a screen:** give its route a `RoutePrefixes` entry on the topic that covers it, or the **?**
falls back to `HelpTopics.DefaultTopicId`. Prefixes are matched by whole segment, longest wins — that
is how `dataset/blueprint` beats `dataset` for `/dataset/Blueprint/3`.

**Tests are the tripwire.** `HelpServiceTests` runs against the real embedded content and fails when a
topic has no Markdown, a page has no `<h1>`, or a cross-page link names a topic that does not exist.
`HelpProcessorTests` covers route resolution and the link/front-matter rewriting.

### Voice

Written for players, not developers: playful, second person, framed around real survival crafting games
(Minecraft, Rust, Valheim, Conan Exiles, No Man's Sky). The repo's "explain with code" rule is for
conversation about the code and does not apply here — help pages explain with worked examples and plain
language, never with C#. Read two existing pages before writing a third.

The pages were rewritten by hand in September 2026 to sound less like documentation. What that edit pass
actually changed, so the next page matches instead of regressing:

**Contract the verbs.** `does not` → `doesn't`, `there is` → `there's`, `you have decided` → `you've
decided`. Formal auxiliaries are the single loudest tell that Claude wrote a page.

**Break the em-dash habit.** Em dashes are allowed, but rarely; a colon, a semicolon, or a full stop is
almost always the better cut. Definition bullets and callout labels take a colon, not a dash:

```markdown
- **Follow system** — matches your phone or desktop.   <!-- before -->
- **Follow system:** matches your phone or desktop.    <!-- after -->
```

```markdown
It is a short screen. Here is all of it.               <!-- before -->
It's a short screen. Here's everything you need to know.   <!-- after -->
```

**The author is a person, and it is fine to hear them.** First person singular carries the jokes and the
honest asides — "I'm not your mom", "I could keep going, but I think you get the point", "who doesn't
care about money, amiright?". Never "we": `we recommend` is a corporate voice the pages do not have.

**Plain words beat precise ones.** "crap", "junk", "stuff", "grindy", "a buttload of units" are in voice.
"granularity", "arbitrary", "leverage", "utilize" are not.

**Explain the payoff, not just the field.** Almost every section gained a sentence saying why the reader
should bother, usually in terms of grind avoided: "The numbers don't have to be exact. Even a rough
estimate can give you a better idea of how long it will take to collect 1000 of these than just leaving
it at 0."

**Examples are real recipes with real numbers, and the arithmetic has to hold.** Minecraft's 1 log → 4
planks, Valheim's 2 Copper + 1 Tin Bronze, Rust's 25 wood + 10 stone → 2 arrows. The Bronze Axe example
is carried across `getting-started.md` and `calculations.md`, so a number changed in one page has to be
changed in the other — check the totals, the crafts, and the step count when you touch either.

**Structural conventions the pages now share:**

- A navigation path names the icon on every step:
  `![Dataset](assets/menu-book.svg) **Dataset** → ![Components](assets/inventory-2.svg) **Components** → ![New](assets/add.svg) **Add**`
- A screen is a link on first mention in a section: `[Craft screen](craft-screen.md)`.
- A section a whole page covers ends by sending the reader there: `More detail: [Components](components.md).`
- Asides are `> **Note:**`, `> **Tip:**` or `> **Example:**` blockquotes, not parentheses. An example that introduces
  a table or a code block is the exception: a bare `**Example:**` lead, no blockquote.
- Troubleshooting headings are the complaint in the reader's own words: `### "It wants way more
  materials than the game does"`.
- Prose wraps at about 120 columns. Do not reflow a paragraph you are making a two-word fix to.
- American English throughout: gray, license, color, modeling, summarized.

**A hard line break is two trailing spaces. Never a backslash.** The three renderers disagree: CommonMark
(Markdig, GitHub) reads a single trailing `\` as a break and `\\` as a literal backslash, while kramdown
(Jekyll, so the project site) reads `\\` as the break and a single `\` as literal. Two or more trailing
spaces are a break in all three. `[*.md]` in `.editorconfig` sets `trim_trailing_whitespace = false` so a
reformat cannot silently eat one — which also means a stray double space anywhere becomes an unintended
`<br>`. `grep -rn --include=*.md -e '  $' docs/help` lists every break in the content.

### MudBlazor docs MCP server (`mudblazor` / MudMCP)

A local [MudMCP](https://github.com/mcbodge/MudMCP) server can index the **MudBlazor 9.7.0** source
and serve component docs, code examples, and API reference (component list, prop/parameter lookup,
semantic search).

- **Use it for any MudBlazor question** — prefer it over recalled knowledge for component parameters,
  slots, enums, and breaking changes between MudBlazor major versions. Keep the version in sync with
  the `MudBlazor` entry in [Directory.Packages.props](../../../Directory.Packages.props) (currently
  `9.7.0`) — versions are centrally managed, so that file is the single source of truth. **Re-index
  after a MudBlazor bump**, otherwise the server answers from the old version.
- Its tools usually arrive as **deferred tools**: load their schemas with `ToolSearch` (query
  `mudblazor`) before calling them. If they aren't present at all, the server just needs registering
  (see below) and a Claude Code restart — a skill can't start an MCP server itself.
- **Setup / maintenance** (machine-local, not committed): repo at `Z:/Repos/MudMCP`, binary at
  `publish/win-x64/MudBlazor.Mcp.exe`.
  - **Registration is machine-local** (`claude mcp add -s local`), never a committed `.mcp.json` — the
    binary path differs from machine to machine. Register it **from `cmd.exe`**, not PowerShell:
    ```
    claude mcp add mudblazor -s local -- "Z:/Repos/MudMCP/publish/win-x64/MudBlazor.Mcp.exe" --stdio --version 9.7.0
    ```
    **This fails in PowerShell** — including Rider's and IntelliJ's integrated terminals — with
    `unknown option --stdio`. PowerShell does not honor a bare `--` as a POSIX end-of-options
    separator, so `claude` keeps parsing the arguments meant for the server (`--version` is a real
    `claude` flag, which is the likely trigger). It is not a server problem: the published binary reads
    `args.Contains("--stdio")` and strips it before handing the rest to the host builder, and its own
    usage text advertises `--stdio`. Verified working in `cmd.exe`. To run it from PowerShell anyway,
    use the stop-parsing token:
    ```powershell
    claude --% mcp add mudblazor -s local -- "Z:/Repos/MudMCP/publish/win-x64/MudBlazor.Mcp.exe" --stdio --version 9.7.0
    ```
  - Check status: `claude mcp list` (expect `mudblazor: … ✓ Connected`) after restarting Claude Code.
  - MudMCP also runs as an HTTP server on `http://localhost:8000/mcp` if the stdio route gives trouble.
  - First launch of a new `--version` clones MudBlazor + builds an index (slow); warm it once by
    running the exe directly before relying on it via MCP.
  - On the Mac the path differs — register it there separately, with the same version.

## Building & testing

- Build a specific testable project (faster than the MAUI head, which needs platform workloads):
  ```
  dotnet build src/CraftingCalculator.Application/CraftingCalculator.Application.csproj
  ```
- Run tests: `dotnet test` (or target `tests/CraftingCalculator.Application.UnitTests/...`).
- **`CraftingCalculator.Tests.slnf`** filters the solution down to Domain / Application /
  Infrastructure plus the two test projects — everything except the `CraftingCalculator.UI` MAUI head.
  Use it (`dotnet build CraftingCalculator.Tests.slnf`, `dotnet test CraftingCalculator.Tests.slnf`)
  on any machine without the MAUI workloads, and in CI. **Add every new non-MAUI project to it** as
  well as to the .sln, or CI silently stops building it.
- **`.editorconfig` conformance is a CI gate.** The `Unit Tests` workflow runs
  `dotnet format CraftingCalculator.Tests.slnf --verify-no-changes` before it builds, so anything
  `dotnet format` would rewrite — file-scoped namespaces, a UTF-8 BOM, whitespace — fails the build.
  Run `dotnet format CraftingCalculator.Tests.slnf` before pushing. **EF-generated migrations are in
  scope:** `dotnet ef migrations add` emits a BOM and a block-scoped namespace, so format the new
  migration (its `Up`/`Down` operations are untouched by that) rather than excluding it.
- **CI is three workflows.** `unit-tests.yml` is the `core` job above on `ubuntu-latest`, triggered
  by every PR into `main` — the only automatic gate. `build.yml` holds one Release build per platform
  head (`windows`, `android` → APK artifact, `ios` → compile check with `-p:CodesignKey=""`) and is
  **`workflow_dispatch` only** until the app is finished, when it becomes a tag-triggered
  build/release pipeline; those jobs build `-c Release` on purpose — trimming, linker and interpreter
  failures do not exist in Debug. `codeql-analysis.yml` builds the same `.slnf` with `build-mode:
  manual`, because CodeQL autobuild does not handle the MAUI head.
- **Test stack:** NUnit + Moq + AwesomeAssertions (`result.Should()...`; the Apache-2.0 community fork
  of FluentAssertions, which went to a paid Xceed license at v8 — do not add `FluentAssertions` back).
  Tests mirror source folders. Mock DAOs and inject them into the service under test.
- **`Infrastructure.UnitTests` uses a real SQLite database, never `UseInMemoryDatabase`.** The
  in-memory provider enforces neither foreign keys nor cascade delete — exactly the mechanisms the
  delete-behavior configuration relies on. Use a fixture that opens
  `DataSource=:memory:;Cache=Shared` with a held-open connection, runs `Migrate()`, and hands back an
  `IDbContextFactory`.
- Building the full `CraftingCalculator.UI` MAUI head requires the MAUI workloads + platform SDKs
  (Android/iOS/Windows). Prefer building/testing the non-MAUI projects when verifying logic changes.

### Environment gotchas
- All projects target **net10.0**, so the **.NET 10 runtime must be installed** to run tests
  (`winget install Microsoft.DotNet.Runtime.10`). Without it the test host fails to launch (`You must
  install or update .NET to run this application`).

## When adding a feature (typical checklist)

1. Entity → `Domain/Entities` (+ enum/constant if needed); domain model → `Domain/Models` if the UI
   needs a shape distinct from the entity. 2. Add `DbSet` / an `IEntityTypeConfiguration<T>` in
   `Infrastructure/Data/Configurations` if persisted. 3. EF migration (+ seed SQL only for the initial
   baseline — later data changes are migrations, not seed edits). 4. DAO interface in
   `Application/Common/Interfaces/DAO` + impl in `Infrastructure/DAO/Impl` → register in
   `Infrastructure/DependencyInjection.cs`. 5. Service interface + impl in
   `Application/Common/Services` → register in `Application/DependencyInjection.cs`. 6. Transformation
   logic in a `Processor`. 7. Razor page/component in `UI/Components` injecting the service, mobile-
   first per the design rules above. 8. Unit tests mirroring the source path. 9. **Update the help
   pages under `docs/help/` for anything the change makes visible to the user**, and add a
   `HelpTopics` entry if the feature introduces a new screen — see "Built-in help" above.
