---
title: Favorites
nav_order: 9
---

# Favorites

A favorite is a saved batch: a set of blueprints with their quantities, stored with a name you choose.

The use case writes itself. You've worked out the exact shopping list for a full raid kit, the stack of stasis 
devices that your farms produce enough resources for, or exactly how to build the base you're going to use. You're 
going to want that same list on the next server, the next wipe, the next character, the next time you want to make a 
buttload of units. Save it once, load it in a tap.

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
