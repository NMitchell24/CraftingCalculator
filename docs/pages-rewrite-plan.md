# Project site rewrite — plan

**Status:** not started. Written 2026-09-09, alongside the built-in help feature.

The site under `docs/`, published at <https://nmitchell24.github.io/CraftingCalculator/>, still
describes the **1.x WPF desktop app** built for No Man's Sky: its copy, feature list, screenshots,
`CraftingCalculator.zip` download and release notes all predate the MAUI rewrite. This plan is what
replaces it, and the reason it is worth doing now rather than later is that **the content already
exists**: `docs/help/*.md` is the manual for the current app, and it is already sitting in the
directory GitHub Pages serves.

## What is already true

`docs/help/` was built as a single source with three consumers in mind. Two are live:

| Consumer | Status |
|---|---|
| The app | Live. `CraftingCalculator.Application.csproj` embeds the `.md` files; `HelpService` + Markdig render them; `UI/Components/Pages/Help.razor` displays them. |
| The GitHub wiki | Live. `.github/workflows/help-sync.yml` publishes on merge to `main`. |
| **The project site** | **Not wired up.** The files are in `docs/`, with `title` and `nav_order` front matter, waiting. |

The content was authored to stay portable, and that constraint has to survive the rewrite: **plain
Markdown, no raw HTML, no Liquid, cross-page links written as `other-page.md`, no external links.** Any
site design that needs the Markdown changed is the wrong design — put the work in the layout instead.

The pages also reference the app's own control icons as relative images (`assets/delete.svg`, the
Material Design icons under `docs/help/assets/`). Jekyll copies those straight through, so they need no
plugin — but the layout should account for them: they carry a fixed mid grey so the same file works on
both GitHub themes, and a site with a dark mode may want `filter: invert()` on `.help-icon` rather than
a second set of files.

## The work

### 1. Decide the Pages source

Check **Settings → Pages** first. The repo has a `_config.yml` at the **root** while the site content
is under `docs/`, which cannot both be right — a `/docs` source reads `docs/_config.yml` and ignores the
root one. Settle it before anything else:

- **Source = `/docs`** (recommended). Move the theme declaration to `docs/_config.yml`. Keeps the site
  files and the help content in one directory, and keeps them out of the repo root.
- Source = GitHub Actions, if the design later needs a build step. Not needed for plain Jekyll.

### 2. Turn on `jekyll-relative-links`

This is the piece that makes the reuse work without touching the content. It is on the GitHub Pages
plugin allowlist, so no Actions build is required:

```yaml
# docs/_config.yml
plugins:
  - jekyll-relative-links
relative_links:
  enabled: true
  collections: false
```

It rewrites `[Blueprints](blueprints.md)` to the rendered page's URL. Without it, every cross-page link
in the manual 404s on the site — and the alternative (rewriting the links) breaks GitHub's own
rendering and the wiki.

### 3. Write the layout

One Jekyll layout for a help page, and one for the landing page. What it needs:

- A **sidebar or contents list built from `nav_order`** — the front matter is already there, so
  `site.pages | where_exp | sort: "nav_order"` gives the manual's order without a hand-maintained nav.
- **Dark and light**, matching the app's palette (`UI/Theme/AppTheme.cs`), so the site and the app read
  as one product.
- The two bundled faces — **Inter** for body, **Jersey 20** for display — as `docs/assets/fonts/`, the
  same files as `UI/wwwroot/Fonts/`. Nothing else in the site should introduce a third face.
- **Mobile-first**, same as the app. Most visitors will be on a phone.
- Styles for the same elements the app's `.help-article` block covers: headings, tables that scroll on
  a narrow screen, `pre`/`code`, blockquotes.

### 4. Rewrite the landing page

`docs/index.html` is 1.x. Replace it with a page that:

- describes the **MAUI app** — Android, iOS, Windows; local storage; no account, no network, no
  telemetry;
- links to the manual (`help/welcome.md`) as the primary call to action;
- carries **new screenshots** from the current app, in both themes. The existing
  `docs/assets/img/*.png` are all WPF and should go.

### 5. Deal with the 1.x artefacts

Decide deliberately rather than by omission:

- `docs/assets/resources/CraftingCalculator.zip` — the 1.x download. Either keep it behind an
  "Older versions" link with a clear "this is the retired Windows desktop app" note, or drop it.
- `docs/release-notes.html` — 1.x notes. Replace with notes for the MAUI releases once `build.yml`
  becomes the tag-triggered release pipeline (it is `workflow_dispatch` only today), or archive it.
- `docs/assets/bootstrap/`, `docs/assets/js/script.min.js` — the old template's Bootstrap and jQuery.
  Delete with the template; the new layout should need neither.

### 6. Cross-link it

- README: point at the published manual rather than only at `docs/help/`.
- The app's Settings → About could carry the site URL — but note that the app deliberately has **no
  external links today**, and adding one means handling `Launcher.OpenAsync` rather than letting the
  `BlazorWebView` navigate. That is a small feature of its own, not part of the site work.

## Risks and things to check

- **Jekyll's front-matter requirement.** Pages already have it. Any future help page without it renders
  as raw text on the site while still working in the app — the kind of failure that is invisible until
  someone visits. Consider a CI check if it bites.
- **The `-` in `docs/help/getting-started.md` and friends** is fine as a URL slug, but confirm the
  generated permalinks match what the wiki uses if anything ever links across the two.
- **Do not let the site design leak back into the Markdown.** If a layout needs a hero image, a badge
  row or a call-out box, that belongs in the layout, keyed off front matter — never as HTML inside a
  file the app also renders.

## Definition of done

- The site describes the current app, not 1.x.
- The manual is published from `docs/help/*.md` with no duplicated content anywhere.
- A change to a help page updates the app, the wiki **and** the site, from one commit.
