---
title: Categories
nav_order: 5
---

# Categories

A category is a custom label that you can use to organize your [components](components.md) 
and [blueprints](blueprints.md). 

You link one on any component or any blueprint, and from then on you can filter your lists down to just that label. 
With only a couple records, you may wonder why you should bother. But once your dataset grows to a substantial size, 
you'll wish you had used them in the beginning. 

## Creating one

**Dataset → Categories → ![New](assets/add.svg)**. There are exactly two fields:

- **Name:** required. You can call it whatever you want. For example, you may want categories for `Ores`, `Food`, 
  `Base Building`, `Tier 3`, `Armor`, `Explosives`.
- **Description:** optional. A free-form field to add in any notes you want. This is the only place in the app where 
  you will see them.

That's it. Tap ![Save](assets/save.svg) **Save**.

## Using one

When adding or editing any component or blueprint you have the option to select a **Category**. Pick one, or 
clear it with the **×** to leave the record uncategorized. A record can only be in **one** category at a time.

The category then shows up as a small chip on that record wherever it displays in the app. This allows you to easily 
visually identify what group that item belongs to.

## Filtering by category

Any list with a search bar also gets a ![Filter](assets/filter-list.svg) button next to it, including
the blueprint picker on the Craft screen.

Tap it and you get a checklist of categories. Tick as many as you like; the selection list will then filter to 
records in **any** of the ticked categories. Your active filters appear as chips under the search box, each with an
**×** to drop it.

Two things worth knowing:

- **The menu only offers categories that are actually in use** in the list you are looking at. Offering
  a category nothing is filed under would just clutter your view with no added benefit.
- **Uncategorized** appears as its own entry in a different color, since it is not a category you
  created. It only shows up whenever the list mixes categorized records with records that have none. It is the fastest
  way to find records you meant to label and never did.

The filter button is hidden entirely on lists where nothing is categorized, and on the Categories list
itself, which would be a bit redundant.

## Naming schemes that work well

Pick one axis and stay on it. Mixing several is what makes a dataset confusing. It's best to stay with categories 
that make sense for the game you are playing.

| Scheme          | Looks like                              | Good for                                                      |
|-----------------|-----------------------------------------|---------------------------------------------------------------|
| **By material** | Ores, Wood, Cloth, Hide                 | Games with clear material tiers such as Valheim, Conan Exiles |
| **By purpose**  | Building, Weapons, Armor, Food, Medical | Rust, where you think in loadouts                             |
| **By tier**     | Tier 1, Tier 2, Tier 3, Endgame         | If you like to think about your games in terms of progression |

## Deleting a category

Deleting a category does **not** delete the components and blueprints filed under it. They survive; they
simply become uncategorized, and you can recategorize them whenever you like.

The ![Delete all](assets/delete-forever.svg) **Delete all Categories** action clears the lot in one go,
after a confirmation. Same deal. Your blueprints and components are untouched.
