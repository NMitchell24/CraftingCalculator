---
title: Favorites
nav_order: 11
---

# Favorites

A favorite is a saved batch: a set of blueprints with their quantities, stored with a name you choose.

The use case writes itself. You've worked out the exact shopping list for a full raid kit, the stack of stasis 
devices that your farms produce enough resources for, or exactly how to build the base you're going to use. You're 
going to want that same list on the next server, the next wipe, the next character, the next time you want to make a 
buttload of units. Save it once, load it in a tap.

A favorite belongs to the [dataset](managing-datasets.md) it was saved in, the same as everything else.
Switch datasets, and you'll see that dataset's favorites instead.

## Saving one

Build your batch on the [Craft screen](craft-screen.md), then tap ![Save as favorite](assets/save.svg)
**Save as favorite**.

- If the batch is empty, nothing happens. There's nothing to save.
- If you built the batch fresh, you're asked for a name.
- If that name is already taken, you're asked whether to overwrite it.
- If you *loaded* a favorite and then changed it, you get a three-way choice: **Update** the one you
  loaded, **Create New** under a different name, or **Cancel**.

> **Tip:** Do you have similar batches for different tiers of gear or different quantities for the same blueprints? 
> Load one, bump a couple of quantities and/or add a few extra blueprints and then save as a new favorite.

## Loading one

- Select your named favorite in **The Load Favorite dropdown** at the top of the Selected Blueprints card on the 
  Craft screen.
- If you already have blueprints in your batch, the app stops and asks before replacing them; one mis-tap on a phone 
  shouldn't cost you.

## The Favorites screen

A list of everything you've saved, each row showing its name and how many blueprints it holds.

| Control                                        | What it does                     |
|------------------------------------------------|----------------------------------|
| **Tap the row**, or ![Rename](assets/edit.svg) | Rename it.                       |
| ![Delete](assets/delete.svg)                   | Delete it, after a confirmation. |

Names need to be unique. You can't create two favorites called `Raid Kit`. If the favorite you're renaming is the 
one currently loaded on the Craft screen, the Craft screen updates to the new name.

There is no **New** action here, and that's deliberate: a favorite is made *from* a batch, and the
batch lives on the Craft screen. Build it there, save it there. This screen just exists for you to manage the 
favorites you create.

### The actions

Two [actions](actions-bar.md), for when deleting one row at a time gets old. They work exactly the way they do on
the [Dataset](dataset.md) lists.

| Action                                                  | What it does                               |
|---------------------------------------------------------|--------------------------------------------|
| ![Delete](assets/delete.svg) **Delete**                 | Turns on Delete mode.                      |
| ![Delete all](assets/delete-forever.svg) **Delete all** | Wipes every favorite after a confirmation. |

Both gray out until you've saved a favorite, since there would be nothing to act on.

### Delete mode (deleting several at once)

Tap ![Delete](assets/delete.svg) **Delete**, then tap every favorite you want gone. Selected rows will highlight.
Then tap it a second time to delete all the selected favorites. You get a confirmation naming how many are about to
go. While the mode is on, tapping a row selects it instead of opening the rename dialog.

> **Tip:** Changed your mind? **Press and hold the ![Delete](assets/delete.svg) action** to leave the mode with
> nothing deleted and your selection discarded.

None of this touches your blueprints. You're only throwing away the favorite, not the blueprints.

## What a favorite actually stores

It stores **which blueprints, and how many**. It's **not** a frozen copy of their Crafting Summary, Components, Steps 
and Surplus. If you update the underlying recipes, the craft screen will account for the recipe changes the next 
time you load the favorite.

This is intentional. Fix a wrong quantity in the Iron Nails blueprint, and every favorite that includes it is instantly 
correct too. Nothing else to edit, nothing else to check. If the favorite saved a frozen copy of the 
whole craft screen, then you'd have to recreate the favorite every time the game devs decide to rebalance their recipes.

The flip side of the same coin: if you **delete** a blueprint, it disappears from every favorite that
used it. The favorite survives. It just gets shorter. And **Delete all data** on the Dataset screen
deletes all your favorites too.
