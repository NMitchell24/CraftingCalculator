---
title: Categories
nav_order: 6
---

# Categories

A category is a label. That is genuinely the whole feature.

You stick one on a [component](components.md) or a [blueprint](blueprints.md), and from then on you can
filter your lists down to just that label. With twelve records you will wonder why you bothered. With
three hundred — and a serious Conan Exiles or modded Minecraft dataset gets there fast — it is the
difference between finding a recipe and giving up.

## Creating one

**Dataset → Categories → ![New](assets/add.svg)**. There are exactly two fields:

- **Name** — required. `Ores`, `Food`, `Base Building`, `Tier 3`, `Armor`, `Explosives`.
- **Description** — optional. Notes to yourself.

That is it. Tap ![Save](assets/save.svg) **Save**.

## Using one

Every component and blueprint editor has a **Category** dropdown. Pick one, or clear it with the **×**
to leave the record uncategorized. A record can be in **one** category at a time.

The category then shows up as a small chip next to that record's name — in the Dataset lists, in the
blueprint picker, in the batch on the Craft screen, in the Components and Surplus tabs, and on every
detail card. It is a constant, quiet reminder of what you are looking at.

## Filtering by category

Any list with a search bar also gets a ![Filter](assets/filter-list.svg) button next to it — including
the blueprint picker on the Craft screen, which is where you will use it most.

Tap it and you get a checklist of categories. Tick as many as you like; the list narrows to records in
**any** of the ticked categories. Your active filters appear as chips under the search box, each with an
**×** to drop it.

Two things worth knowing:

- **The menu only offers categories that are actually in use** in the list you are looking at. Offering
  a category nothing is filed under would just be a way to make the screen go blank.
- **Uncategorized** appears as its own entry — in a different colour, since it is not a category you
  created — whenever the list mixes categorized records with records that have none. It is the fastest
  way to find records you meant to label and never did.

The filter button is hidden entirely on lists where nothing is categorized, and on the Categories list
itself, which would be a bit recursive.

## Naming schemes that work well

Pick one axis and stay on it. Mixing several is what makes a dataset unbrowsable.

| Scheme | Looks like | Good for |
|---|---|---|
| **By material** | Ores, Wood, Cloth, Hide | Games with clear material tiers — Valheim, Conan Exiles |
| **By purpose** | Building, Weapons, Armor, Food, Medical | Rust, where you think in loadouts |
| **By tier** | Tier 1, Tier 2, Tier 3, Endgame | Progression-heavy games and modded packs |
| **By game** | Minecraft, Valheim, Rust | One install, several games, one dataset |

That last row is worth calling out. If you play more than one game, tagging every record with its game
turns a single shared dataset into several — filter to `Valheim` and everything else vanishes.

## Deleting a category

Deleting a category does **not** delete the components and blueprints filed under it. They survive; they
simply become uncategorized, and you can re-file them whenever you like.

The ![Delete all](assets/delete-forever.svg) **Delete all Categories** action clears the lot in one go,
after a confirmation. Same deal — your actual data is untouched, it just loses its labels.
