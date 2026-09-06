# CLAUDE.md

Guidance for Claude Code (and humans) working in this repository. This file is always loaded into
context. For deeper detail it points to a companion that travels with the repo:

- **`.claude/skills/craftingcalculator-dev/SKILL.md`** — the canonical architecture/conventions/workflow
  reference (DAO patterns, migration steps, test stack, MudBlazor MCP). Invoke the
  `craftingcalculator-dev` skill when working on the codebase.
- **`docs/dev-environment.md`** — building/running in Rider/VS, Android + Apple device setup, and the
  `global.json` SDK pin.

---

## What this is

**CraftingCalculator** — a helper app for survival crafting games. Users maintain a database of
ingredients and recipes (recipes can nest other recipes), then pick recipes with quantities and see
the total raw materials, cost, value, and profit. A **.NET 10 MAUI Blazor Hybrid** app (Android / iOS /
Windows) using **MudBlazor** for UI and **EF Core + SQLite** for on-device storage. All user data stays
local; the app collects no personal information and requires no account.

---

## Architecture — Clean Architecture, and the choices we made

Dependency direction: **Domain ← Application ← Infrastructure ← UI**. Each layer depends only inward.

| Project | TFM | Role |
|---|---|---|
| `CraftingCalculator.Domain` | net10.0 | Entities, Enums, Models, Constants. No internal deps. |
| `CraftingCalculator.Application` | net10.0 | Business logic, services, **all interfaces** (Service + DAO), Processors. |
| `CraftingCalculator.Infrastructure` | net10.0 | EF Core `DbContext`, DAO implementations, migrations, SQL seed data. |
| `CraftingCalculator.UI` | net10.0-android/ios/windows | MAUI Blazor host: Razor pages/components, DI wiring, platform code. |

**Choices that define how the pattern is applied here — follow them:**

- **Interfaces live in `Application/Common/Interfaces`** (both `IXxxService` and `IXxxDAO`). DAO
  *implementations* live in `Infrastructure/DAO/Impl`. Never put DAO interfaces in Infrastructure —
  that would invert the dependency direction.
- **Layered call flow:** Razor page (`@inject IXxxService`) → Service (`Application/Common/Services/Impl`)
  → DAO (`Infrastructure/DAO/Impl`) → `CraftingDataContext`. Services orchestrate; DAOs do data access
  only; pure transformation logic goes in `Application/BusinessLogic/Processors` (static methods).
- **Entities and models stay dumb.** Logic that operates on them lives in exactly one service or
  processor — not spread across callers (see the "Data Class with leaked logic" smell below).
- **DI is manual and split:** services in `Application/DependencyInjection.cs` (`AddApplicationServices`),
  DAOs + DbContext in `Infrastructure/DependencyInjection.cs` (`AddDatabaseServices`); both wired from
  `CraftingCalculator.UI/MauiProgram.cs`. Register every new service/DAO in the matching file.
- **DAOs use short-lived contexts:** inject `IDbContextFactory<CraftingDataContext>` and
  `await using var context = await contextFactory.CreateDbContextAsync();` per operation. **Never
  register the `DbContext` itself** — `BlazorWebView` holds one `IServiceScope` for the whole app
  session, so a `Scoped` `DbContext` would live for the entire session.
- **Magic strings/enums live in `Domain/Constants` and `Domain`** (e.g. `RecipeFilter.ALL`, the
  currency format string). Use them instead of inline literals.
- **Seed data is SQL** (`Infrastructure/Data/Seed/*.sql`, embedded resources) run once by the initial
  migration to establish the **initial** data state. **Once released, the seed is final — never edit it
  or re-run it.** Any data change now ships as a **new migration** that Inserts, Updates, or Deletes
  rows on top of the seeded state.

See the `craftingcalculator-dev` skill for the full feature-addition checklist and the DAO/service
templates.

---

## Key commands

```bash
# Build a single (non-MAUI) project — fast, no platform workloads needed
dotnet build src/CraftingCalculator.Application/CraftingCalculator.Application.csproj

# Run tests (NUnit + Moq + AwesomeAssertions)
dotnet test

# Build the MAUI head for a specific platform
dotnet build src/CraftingCalculator.UI/CraftingCalculator.UI.csproj -f net10.0-windows10.0.19041.0

# Add an EF Core migration (run from src/CraftingCalculator.Infrastructure). DB auto-migrates on app
# launch. No --startup-project needed: CraftingDataContextFactory (IDesignTimeDbContextFactory)
# builds the context.
dotnet ef migrations add <Name>
```

- **`global.json` pins the SDK to .NET 10.0.x** (`allowPrerelease:false`) so no machine silently
  builds against a preview SDK (see `docs/dev-environment.md`).
- **MudBlazor:** a local MudBlazor-docs MCP server *may* be available — it depends on per-machine
  setup, so don't assume it's present. Check whether its tools are loaded; if so, use it to verify
  component APIs before editing rather than relying on recalled knowledge. If it isn't available, ask
  the user whether they'd like to install and set it up (setup steps are in the
  `craftingcalculator-dev` skill).
- Keep the build **warning-free**. `TreatWarningsAsErrors` is intentionally `false` (blanket
  enforcement is risky across the mobile TFMs); enforce specific codes with `<WarningsAsErrors>` when
  you want to lock one in.

---

## Coding standards

### Explain with code, not prose

The maintainers of this repo read code faster than English. When explaining anything that has a code
representation — a design decision, a trade-off, a bug, an API, a proposed change — show the code
itself and use prose only as connective tissue:

- Lead with the relevant snippet, quoted from the actual repo with `path:line` references — not a
  paragraph describing it.
- Present options and trade-offs as side-by-side code blocks the reader can compare directly, with a
  short comment marking the line where they differ. Let the code carry the comparison; one sentence
  per option for what the code can't show.
- Never describe code indirectly when you can show it. A sentence about what a change does to an API
  is opaque; the call site that now compiles (or no longer compiles), with a one-line comment, is
  immediately legible.
- Show failure modes as code that compiles-but-misbehaves (or the verbatim compiler/test error), not
  as an abstract description of the risk.
- Keep prose for what code cannot express: intent, constraints, and consequences — one or two
  sentences placed next to the snippet they explain.

This governs how you communicate *about* the code in conversation — chat replies, PR descriptions,
review responses. It does not apply to the repo's own artifacts: documentation (this file, READMEs)
and code comments.

### Comments

**XML doc comments** (`/// <summary>`, `<param>`, `<returns>`) describe the contract from the
caller's perspective: what something is for, what guarantees it makes, what the inputs and outputs
mean. They should not document implementation details — how a DAO persists, which helper it delegates
to, what bypass it uses internally — that's noise for someone using the API and rots when the
implementation changes. Don't document what something does *not* do, only what it does. (Exception:
call out security-relevant non-behavior, e.g. "does not validate input.")

**Inline comments** inside method bodies are different: they're for implementation details that aren't
obvious from reading the code, and only when they aren't. A subtle invariant, the reason for an
unusual ordering, a workaround for a specific bug or platform quirk (`#if ANDROID`, a MudBlazor/MAUI
gotcha), a non-obvious choice between two valid approaches — those earn an inline comment.
Self-evident code does not. If you're about to write a comment that restates what the next line does,
delete it.

The split is about **audience**: XML doc is for *consumers* of the API; inline is for *maintainers*
of the body. The rationale for a defensive check, a workaround, or a tricky ordering belongs inline
next to the code that does it — never in the doc comment, even when it explains *why* the method
behaves the way it does. The caller doesn't care that a DAO method returns null because of how the
SQLite query is shaped; they care that it returns null when there's no matching row. The "because"
stays in the body.

Use standard programming vocabulary — the terms from GoF, Fowler's *Refactoring*, and the
.NET / MudBlazor / EF Core docs: delegate, factory, guard clause, invariant, idempotent, race
condition, callback, dispatch. Never literary metaphors or coined phrases ("prologue", "dance",
"journey", "saga"). Reference the actual method/class names involved rather than describing them
indirectly. State cause and effect directly.

Never remove or alter an existing authorship comment — `Created by <name> on <date>`, `@author`, or
similar attribution. Preserve it verbatim (name, accents, emoji, date, punctuation) when editing or
refactoring a file, including when you rewrite the surrounding doc comments, and carry it with the
type if you move that type to another file.

### Avoid these code smells

These are the named smells from Martin Fowler and Kent Beck's catalog in *Refactoring: Improving the
Design of Existing Code*, grouped by Mäntylä's taxonomy — decades of industry consensus on what makes
code hard to change, not house style. Check every diff against this list before presenting it. The bar
for any new abstraction is **YAGNI** and the **Rule of Three**: it must carry information or remove
duplication *today* — not that it might someday.

**Dispensables — code that should not exist**

- **Speculative Generality.** No one-value enums, no parameters every call site passes the same
  constant to, no "seam for a future toggle," no interface with a single implementation created "just
  in case," no config nobody asked for. Build for the current requirement; introduce the
  discriminator or abstraction when the second concrete case exists to design against.
- **Dead Code.** No defensive branches every caller already makes impossible, no unused parameters or
  imports, no commented-out code, no "kept for later" methods without the owner's explicit say-so. A
  genuinely load-bearing guard earns an inline comment saying so — otherwise delete it.
- **Lazy Element.** A class, method, or file that no longer pulls its weight after a refactor gets
  inlined or deleted, not left behind.
- **Duplicated Code.** Before writing new logic, find the existing seam and compose it. Two
  near-identical blocks in sibling classes mean the shared piece was never extracted — extract it to
  the nearest common layer (a `Processor`, a service), not to a new grab bag.
- **Comments as deodorant.** A comment explaining confusing code is a signal to fix the code. The
  Comments section above governs what comments are for.

**Bloaters — things that have grown past one responsibility**

- **Long Function.** A method doing several things at different levels of abstraction gets decomposed,
  each piece named for what it does.
- **Large Class / junk drawers.** The smell is a class accumulating *unrelated* members that changes
  for many reasons; split it along the reasons it changes. A cohesive, stateless, well-named static
  helper (`IngredientProcessor`, `Domain/Constants`) is fine — a single known home beats scattering.
  Don't overcorrect: a file created to hold a single static method is a Lazy Element; fold it into the
  nearest cohesive home.
- **Long Parameter List / flag arguments.** A boolean or mode parameter that forks a method's whole
  behavior is two methods. More than ~4 parameters is a sign some of them are a missing type.
- **Data Clumps.** Values that always travel together belong in one type (a `Domain/Models` record),
  not loose parameter pairs repeated across signatures.
- **Primitive Obsession.** Prefer the enums/constants in `Domain` over bare strings/ints for ids,
  keys, and category/filter names used across boundaries.

**Couplers — classes that know too much about each other**

- **Middle Man / needless indirection.** No wrapper or dispatch layer with a single consumer — inline
  it until at least two real consumers exist. A flow should be followable inside one class. Same for
  constants: used in one class → declared there; shared catalogs (`Domain/Constants`) only for genuine
  cross-class/cross-layer contracts.
- **Feature Envy.** A method that mostly reads and combines another class's data belongs on that
  class (or its service).
- **Inappropriate Intimacy.** Don't reach through another class's internals — go through its
  interface. Crossing the layer boundaries the wrong way (UI or a DAO bypassing the `Application`
  service interfaces; a DAO interface placed outside `Application/Common/Interfaces`) is this smell by
  definition.
- **Message Chains.** `a.GetB().GetC().GetD()` couples the caller to the whole path. Ask the nearest
  object for what you actually need.

**Change preventers — structure that makes edits expensive**

- **Shotgun Surgery.** When one logical change forces edits in many files (a filter name, a setting
  key, a route), the knowledge is scattered — give it a single home (a `Domain/Constants` entry)
  first, then change it once.
- **Divergent Change.** When one class keeps changing for unrelated reasons, split along the reasons it
  changes.
- **Repeated Switches.** The same `switch`/`if-else` over the same discriminator in more than one
  place means polymorphism or a map is missing. One occurrence is fine; the second copy is the smell.

**Object-orientation abusers**

- **Alternative Classes with Different Interfaces.** Two classes doing the same job must share a shape:
  same method names, same parameter order.
- **Refused Bequest.** Don't extend a base class to use one method while ignoring the rest — compose
  instead.
- **Temporary Field.** A field only meaningful during one operation is a missing parameter or small
  object, not state.
- **Data Class with leaked logic.** Entities/models stay dumb (our convention), but the logic operating
  on them must then live in ONE service/processor — not spread across every caller.

### Working on existing code that already has these smells

The smell catalog above is the bar for **new and changed** code. It is **not** a license to refactor
unrelated existing code mid-task. When you encounter code that already exhibits one of these smells
while doing other work, **do not silently refactor it.** Instead:

1. **Surface it.** Tell the user the smell exists, name it, and point to it with `path:line`.
2. **State the impact** of correcting it — what would change, what call sites/tests/behavior it would
   touch, how far the change reaches.
3. **State the risk** of the refactor — what could break, how well it's covered by tests, whether it's
   a localized change or a Shotgun-Surgery-style ripple.
4. **Let the user decide.** They choose whether to (a) fix the smell as part of the current work, or
   (b) leave the code as-is and satisfy the current need with the smell in place. Default to the
   minimal change that meets the current need unless the user opts into the refactor.

The goal is to keep the catalog from turning every task into an unbounded refactor, and to keep
refactor scope and timing decisions with the user.
