## Crafting Calculator

A helper app for survival crafting games. Keep a dataset of components and blueprints — blueprints can
nest other blueprints — then pick blueprints with quantities and see the total raw components, cost,
value and profit for the whole batch.

A **.NET 10 MAUI Blazor Hybrid** app for Android, iOS and Windows, built with **MudBlazor** and
**EF Core + SQLite**. Everything is stored in a local database on the device: no account, no network
calls, no personal information collected.

[![Unit Tests](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/unit-tests.yml/badge.svg)](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/unit-tests.yml)
[![CodeQL](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/codeql-analysis.yml)
[![Dependencies](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/dependencies.yml/badge.svg)](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/dependencies.yml)

### Privacy and Free Software Commitment

Crafting Calculator is published by Sterling Turd Productions, LLC, and every version it publishes keeps these
promises. Not just this release, and not until the next business plan comes along. Forever:

1. **Free, forever.** The app costs nothing to download and nothing to use. No price tag, no subscription, no
   premium tier, no in-app purchases, and no features held back behind a paywall.
2. **No advertisements. Ever.** Not of any kind: no banners, no pop-ups, no sponsored content, no "watch a video to
   unlock."
3. **No telemetry. Ever.** The app contains no analytics, no usage tracking and no crash reporting, and it will
   never include a third-party SDK that does any of those.
4. **No data collection. Ever.** Your datasets, favorites and settings stay in the database on your device.
   Nothing about you or your data is collected, sold, shared or sent anywhere without your explicit permission. The 
   only ways your data leaves the device are an export file you create and share yourself, and your device's own
   backup (Google or iCloud), if you've turned it on. That backup lives in your account. Sterling Turd Productions,
   LLC never sees it.
5. **Your files, your choice.** The app only reads or writes a file outside its own storage when you pick that file
   yourself in your device's file or share dialog.

> **What the app stores can see:** Google Play, the App Store and the Microsoft Store show every developer some
> statistics whether the app asks for them or not, like install counts, and crash reports from people who turned on
> diagnostic sharing in their device settings. That comes from your device and the store, not from code in this
> app, and none of it includes what's in your datasets. Sterling Turd Productions, LLC can't switch that off, but
> it will never add anything on top of it.

These promises are written into section 3 of the [license](LICENSE.md), and they bind every build Sterling Turd
Productions, LLC publishes. The same license reserves the app stores and marketplaces to Sterling Turd Productions,
LLC alone, so any copy you get from one of them keeps these promises. A build someone publishes from their own fork
on GitHub is theirs, not ours. If one of our builds ever breaks a promise, that is a bug: open an issue and hold us
to it.

"Free" here means free of charge. The source is public so anyone can verify these promises are kept, but it is
source-available under a personal-use license, not open source. See [License](#license).

### Screens

- **Craft** — assemble a batch of blueprints with quantities, and see the pinned cost / value / profit
  totals, the combined component list and the full breakdown tree. Copy the totals to the clipboard.
- **Favorites** — save the current batch under a name, then load, rename or delete it later.
- **Dataset** — browse, search and edit the three record types: components, blueprints and categories.
- **Settings** — light / dark / follow-system theme, and About.
- **Help** — the built-in manual, opened from the **?** in the app bar. Context sensitive: it opens the
  page for whichever screen you are on.

### Help content — read this before changing a screen

**The help pages are part of the app, and a change that alters what the user sees is not finished until
they say so.** The rule is short:

> If a pull request changes a screen, a control, a field, a calculation, an action or a default, it also
> changes the matching page under [`docs/help/`](docs/help). No exceptions for "small" changes — a
> renamed button is exactly the kind of thing that makes a help page a liar.

`docs/help/*.md` is the **single source** for that content, and three places publish it:

| Consumer | How it gets there |
|---|---|
| **The app** | `CraftingCalculator.Application.csproj` embeds `..\..\docs\help\*.md` as resources; `HelpService` renders them with Markdig and `Components/Pages/Help.razor` displays them. |
| **The GitHub wiki** | `.github/workflows/help-sync.yml` pushes them on every merge to `main`, stripping the front matter and the `.md` from links. |
| **The project site** | The files sit under `docs/`, front matter and all, ready for Jekyll. Not wired up yet — see [docs/pages-rewrite-plan.md](docs/pages-rewrite-plan.md). |

Because of that, the content has to stay portable:

- **Author in plain Markdown.** No raw HTML, no Jekyll Liquid tags, no MudBlazor markup — the same file
  has to render on GitHub, in the wiki and in the app.
- **Link between pages as `other-page.md`**, optionally with an anchor (`calculations.md#surplus`).
  GitHub and the wiki resolve that themselves; the app rewrites it to `/help/other-page`.
- **No external links.** The app runs inside a `BlazorWebView` with nowhere to send them.
- **Name a control with its icon**, as an ordinary image: `![Delete](assets/delete.svg)`. Those files
  are the Material Design icons the app itself draws; GitHub, the wiki and Pages render the file, and
  the app inlines its markup so it picks up the current theme color. Icons are the only images help
  content may use.
- **Every page needs front matter** with `title` and `nav_order`.
- **Adding a page** means adding the `.md` *and* an entry in `Domain/Constants/HelpTopics.cs`, which is
  what puts it in the contents list and maps it to an app route. `HelpServiceTests` fails if a topic has
  no file, or if any cross-page link points at a topic that does not exist.
- **Adding a screen** means giving its route a `RoutePrefixes` entry on some topic, or the **?** button
  falls back to the Welcome page.

The tone is deliberate: playful, aimed at players rather than developers, and written around real
survival crafting games. Match it.

### Solution layout

Clean Architecture; dependency direction is Domain ← Application ← Infrastructure ← UI.

| Project | TFM | Role |
|---|---|---|
| `src/CraftingCalculator.Domain` | net10.0 | Entities, models, enums, constants |
| `src/CraftingCalculator.Application` | net10.0 | Services, processors, and **all** interfaces (service + DAO) |
| `src/CraftingCalculator.Infrastructure` | net10.0 | EF Core `DbContext`, DAO implementations, migrations, seed SQL |
| `src/CraftingCalculator.UI` | net10.0-android/ios/windows | MAUI Blazor host: Razor pages, DI wiring, platform code |

`tests/` mirrors the two lower layers: `CraftingCalculator.Application.UnitTests` (NUnit + Moq +
AwesomeAssertions) and `CraftingCalculator.Infrastructure.UnitTests` (the same stack against a real
SQLite database).

### Building

`CraftingCalculator.Tests.slnf` filters the solution down to everything except the MAUI head, so it
restores, builds and tests on a machine with no MAUI workloads or platform SDKs. It is what the
`Unit Tests` workflow and CodeQL build.

```bash
dotnet build CraftingCalculator.Tests.slnf
dotnet test CraftingCalculator.Tests.slnf
dotnet format CraftingCalculator.Tests.slnf --verify-no-changes
```

The app itself builds per platform. Release is the configuration that matters for the mobile heads —
trimming, linker and interpreter failures do not exist in Debug:

```bash
dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -f net10.0-android -c Release
dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -f net10.0-windows10.0.19041.0 -c Release
dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -f net10.0-ios -c Release   # macOS only
```

### The export file format

Datasets export to `.ccdata` files, and **every file a released build has ever written has to import into
every later build.** Adding a column or a table to an exported record therefore follows a fixed procedure: a
defaulted member on the document types and a regenerated fixture export that CI imports forever, plus a
format version bump when the current version has already shipped (at most one per release). The steps, with
code, are in [docs/transfer-format-maintenance.md](docs/transfer-format-maintenance.md).
`TransferSchemaTripwireTests` fails when an exported entity changes, which is the prompt to go there.

`global.json` pins the SDK to stable .NET 10. Running the app from an IDE, the Android JDK/SDK
requirements and the Apple toolchain setup are in [docs/dev-environment.md](docs/dev-environment.md);
architecture and code conventions are in [CLAUDE.md](CLAUDE.md) and the `craftingcalculator-dev`
skill under `.claude/skills/`.

### The 1.x Windows desktop app

Version 1.x was a WPF desktop application built for [No Man's Sky](https://www.nomanssky.com/) and
distributed as a standalone Windows executable. Its project was deleted when this rewrite landed.

The project site under `docs/` — published at <https://nmitchell24.github.io/CraftingCalculator/> —
still describes that build: its copy, feature list, screenshots, download zip and release notes are
all 1.x and predate the MAUI app. Treat it as an archive of the desktop release until it is rewritten.
The plan for that rewrite, which folds `docs/help/` into the site so the manual is published from the
same source the app reads, is [docs/pages-rewrite-plan.md](docs/pages-rewrite-plan.md).

### License

Custom Source-Available and Personal Use License — see [LICENSE.md](LICENSE.md). Copyright © 2026 Sterling Turd
Productions, LLC.

Fork it, modify it, compile it and make it your own, for **personal, non-commercial use**. Forks can live
in public repositories, as long as they keep the license and copyright notice, and you can publish your builds of the
app or your fork on GitHub, as build artifacts or GitHub Releases. What you can't do is sell the software or any fork
of it, or publish the app or a modified build of it to an app store, marketplace or any distribution platform other
than GitHub. Only Sterling Turd Productions, LLC publishes the app to the app stores.

Contributions are welcome, and submitting one grants Sterling Turd Productions, LLC a license to use it (section 4).
Third-party libraries and fonts keep their own licenses (section 5).
