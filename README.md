## Crafting Calculator

A helper app for survival crafting games. Keep a dataset of components and blueprints — blueprints can
nest other blueprints — then pick blueprints with quantities and see the total raw components, cost,
value and profit for the whole batch.

A **.NET 10 MAUI Blazor Hybrid** app for Android, iOS and Windows, built with **MudBlazor** and
**EF Core + SQLite**. Everything is stored in a local database on the device: no account, no network
calls, no personal information collected.

[![Build](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/build.yml/badge.svg)](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/build.yml)
[![CodeQL](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/NMitchell24/CraftingCalculator/actions/workflows/codeql-analysis.yml)

### Screens

- **Craft** — assemble a batch of blueprints with quantities, and see the pinned cost / value / profit
  totals, the combined component list and the full breakdown tree. Copy the totals to the clipboard.
- **Favorites** — save the current batch under a name, then load, rename or delete it later.
- **Dataset** — browse, search and edit the three record types: components, blueprints and categories.
- **Settings** — light / dark / follow-system theme, and About.

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
restores, builds and tests on a machine with no MAUI workloads or platform SDKs. It is what CI's
`core` job and CodeQL build.

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

### License

GPL 2.0 — see [LICENSE](LICENSE). Copyright © Sterling Turd Productions, LLC.
