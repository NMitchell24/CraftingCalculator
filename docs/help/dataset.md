---
title: The Dataset Screen
nav_order: 7
---

# The Dataset Screen

Your library. Everything the app knows about your game lives here, in three lists.

The landing page shows one card per record type, with a live count of how many you have:

| Card | What is in it |
|---|---|
| ![Categories](assets/label.svg) **Categories** | Your labels. See [Categories](categories.md). |
| ![Components](assets/inventory-2.svg) **Components** | Raw materials. See [Components](components.md). |
| ![Blueprints](assets/handyman.svg) **Blueprints** | Recipes. See [Blueprints](blueprints.md). |

They are in that order for a reason: it is the order you have to create them in. A blueprint needs
components to exist first, and a component gets filed under a category. Read the page top to bottom and
you are reading the path a new dataset takes.

Tap a card — anywhere on it — to open that list. The ![Edit](assets/edit.svg) does the same thing.

The page's actions do the same job from the actions bar, plus one extra: **Delete all data**, covered at
the bottom of this page.

## Inside a list

Every row shows the record's name, a caption, and its category chip.

The caption changes with the type: components show their cost, blueprints show how many parts they
have, categories show their description.

**Tap a row to edit it.** ![Edit](assets/edit.svg) does the same; ![Delete](assets/delete.svg) deletes
that one record after a confirmation.

### Search and filter

The ![Search](assets/search.svg) bar at the top filters as you type, matching anywhere in the name — so
`iron` finds `Wrought Iron`. The **×** clears it.

Next to it, ![Filter](assets/filter-list.svg) narrows by category. See
[Filtering by category](categories.md#filtering-by-category).

### The actions

| Action | What it does |
|---|---|
| ![New](assets/add.svg) **New …** | Opens an empty editor for this record type. |
| ![Duplicate](assets/content-copy.svg) **Duplicate** | Turns on Duplicate mode. |
| ![Delete](assets/delete.svg) **Delete** | Turns on Delete mode. |
| ![Delete all](assets/delete-forever.svg) **Delete all …** | Wipes this list after a confirmation. |

The last three grey out when the list is empty, since there would be nothing to act on.

### Duplicate mode

Tap ![Duplicate](assets/content-copy.svg) **Duplicate**, then tap any row. The editor opens on a
**copy** of that record, named `Whatever - Copy`, with nothing saved yet. Change what you need, fix the
name, tap **Save**.

This is the fast way to build out an armour set or a tool tier, where five recipes differ by one
material. Tap ![Duplicate](assets/content-copy.svg) again to leave the mode without doing anything.

### Delete mode (deleting several at once)

Tap ![Delete](assets/delete.svg) **Delete**, then tap every row you want gone — selected rows highlight.
Then tap it a second time to commit. You get a confirmation naming how many records are about to go.

Changed your mind? **Press and hold the ![Delete](assets/delete.svg) action** to leave the mode with
nothing deleted and your selection discarded.

## Editing a record

Tapping a row opens its editor. The fields differ by type — see [Blueprints](blueprints.md),
[Components](components.md) and [Categories](categories.md) — but the bar at the bottom is always the
same:

- ![Delete](assets/delete.svg) **Delete** (on the left, and only on a record that already exists) —
  removes it, after asking.
- **Cancel** — goes back without saving.
- ![Save](assets/save.svg) **Save** — commits. It stays greyed out until the record has a name, because
  a nameless record is unfindable.

**Your edits are safe from a stray tap.** If you have unsaved changes and you hit Cancel, the back
arrow, or a navigation button, the app stops and asks whether to discard them.

## What deleting actually does

Records reference each other, so it is worth knowing what a delete ripples into:

| You delete… | …and this happens |
|---|---|
| A **category** | Its components and blueprints survive, and become uncategorized. |
| A **component** | It is removed from every blueprint that used it. Those blueprints survive. |
| A **blueprint** | It is removed from every blueprint that nested it, and from every favorite that included it. |

Nothing here is undoable, which is why every one of them asks first.

## Delete all data

The ![Delete all data](assets/delete-forever.svg) action on the Dataset landing page, and the big one.
It removes **every** favorite, blueprint, category and component — the entire dataset, gone.

Because that is unrecoverable, it does not settle for a yes/no box: you have to **type the word
DELETE** to confirm. If a batch was in progress on the Craft screen, it is cleared too, since it would
otherwise be pricing out blueprints that no longer exist.

Use it when you are switching games and want a clean slate. Do not use it because a list looks
cluttered — **Delete all Components**, inside a single list, is the smaller hammer.
