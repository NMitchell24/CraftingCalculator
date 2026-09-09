---
title: Favorites
nav_order: 8
---

# Favorites

A favorite is a saved batch: a set of blueprints *with their quantities*, kept under a name you choose.

The use case writes itself. You have worked out the exact shopping list for a full set of Wolf Armor,
or a raid kit, or the twelve stacks of building material a base tier costs. You are going to want that
same list on the next server, the next wipe, the next character. Save it once, load it in a tap.

## Saving one

Build your batch on the [Craft screen](craft-screen.md), then tap ![Save as favorite](assets/save.svg)
**Save as favorite**.

- If the batch is empty, nothing happens — there is nothing to save.
- If you built the batch by hand, you are asked for a name.
- If that name is already taken, you are asked whether to overwrite it.
- If you *loaded* a favorite and then changed it, you get a three-way choice: **Update** the one you
  loaded, **Create New** under a different name, or **Cancel**.

That last one matters. Load `Base Tier 2`, bump a couple of quantities, and you decide on the spot
whether you have improved the recipe or invented a new one.

## Loading one

- Select your named favorite in **The Load Favorite dropdown**, at the top of the Selected Blueprints card on the Craft screen.
- If you already have blueprints in your batch, the app stops and asks before replacing them.
- Loading is destructive to whatever you had selected, and one mis-tap on a phone shouldn't cost you.

## The Favorites screen

A list of everything you have saved, each row showing its name and how many blueprints it holds.

| Control | What it does |
|---|---|
| **Tap the row**, or ![Rename](assets/edit.svg) | Rename it. |
| ![Delete](assets/delete.svg) | Delete it, after a confirmation. |

Renaming checks for collisions — you cannot end up with two favorites called `Raid Kit`. If the favorite
you are renaming is the one currently loaded on the Craft screen, the Craft screen keeps up with the new
name.

There is no **New** action here, and that is deliberate: a favorite is made *out of* a batch, and the
batch lives on the Craft screen. Build it there, save it there.

## What a favorite actually stores

It stores **which blueprints, and how many** — not a frozen copy of their recipes.

This is almost always what you want. Fix a wrong quantity in the Iron Nails blueprint, and every
favorite that includes it is instantly correct too. Nothing to re-save, nothing to re-check.

The flip side of the same coin: if you **delete** a blueprint, it disappears from every favorite that
used it. The favorite survives — it just gets shorter. And **Delete all data** on the Dataset screen
takes the favorites with it.
