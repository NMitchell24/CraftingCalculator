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

The category then shows up as a small chip on that record's detail card, the one ![Info](assets/info.svg) opens. That way you can 
see at a glance what group that item belongs to.

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

### Filters stick around

Every list remembers its own filter. Say you're on Components with `Ores` ticked and you tap into Copper Ore to fix its
cost. When you come back, the list is still showing just your ores. You don't have to tick them again.

Each list keeps its own, and they don't share. That goes for the Blueprints list, the Components list, the picker you
add requirements from in the blueprint editor, and the blueprint picker on the [Craft screen](craft-screen.md).
Filtering the Craft picker down to `Weapons` leaves the Blueprints list alone. That's on purpose: the categories you
care about while picking a batch usually aren't the ones you care about while cleaning up your dataset. The only 
thing that might appear to share filters is the add requirements list when editing blueprints. This list is a shared 
control that lives on every blueprint. So a filter you applied on one blueprint automatically applies on every 
blueprint you edit. This is helpful if you need to edit several blueprints and make similar changes to each one.

The search text doesn't stick. It clears when you leave, same as always.

> **Note:** Filters last until you close the app. Open it again tomorrow and every list starts unfiltered.

If a category you had ticked stops being in use, its chip goes away and the list stops filtering by it. Put something
back in that category before you change that list's filter and the filter comes back with it.

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

The ![Delete all](assets/delete-forever.svg) **Delete all categories** action clears the lot in one go,
after a confirmation. Same deal. Your blueprints and components are untouched.
