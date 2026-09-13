---
title: The Dataset Screen
nav_order: 10
---

# The Dataset Screen

This screen is your library. This is where you will customize the data that you want to use within the Crafting 
Calculator. The Dataset screen allows you to view the three different record types, and a live total of how many you 
have for each. Tap on any card to go to a list of those records where you can view, add, edit and delete them.

## Switching datasets

The droplist at the very top says which dataset you're looking at. Everything below it, and everything
on every other screen, belongs to it. Tap it to switch to another dataset, and use the
![Rename](assets/edit.svg), ![New](assets/add.svg), ![Copy](assets/content-copy.svg) and
![Delete](assets/delete.svg) icons beside it to manage it.

If you only play one game, you can ignore that row entirely and never think about it again.

More detail: [Managing Datasets](managing-datasets.md).

## The record cards

The landing page shows one card per record type:

| Card                                                 | What is in it                                                                              |
|------------------------------------------------------|--------------------------------------------------------------------------------------------|
| ![Categories](assets/label.svg) **Categories**       | Your custom grouping categories. See [Categories](categories.md).                          |
| ![Components](assets/inventory-2.svg) **Components** | Raw materials that are used as crafting components. See [Components](components.md).       |
| ![Blueprints](assets/handyman.svg) **Blueprints**    | Crafting Recipes that you use to make or build something. See [Blueprints](blueprints.md). |

They're in that order for a reason: it's the best order to create them in. A blueprint needs
components to exist first, and both components and blueprints can be linked to a category. Creating these elements 
in this order allows you to make sure that each thing exists before you need to use it. 

Tap the ![Edit](assets/edit.svg) icon to open a list screen for any of the types. You don't have to tap the icon 
directly. Tapping anywhere on the card does the same thing.

> **Note:** There are shortcut icons that match the cards in the [actions bar](actions-bar.md) for easy thumb 
> access on your mobile device. 
> 
> The **![Delete all data](assets/delete-forever.svg) Delete all data** button is 
> covered at the bottom of this page.
>
> The ![Import/Export](assets/import-export.svg) **Import/Export** action opens the screen for backing up and sharing
> your records. See [Import and Export](import-export.md).

## Inside a list

Every row shows the record's name, a caption, and its category chip.

The caption changes with the type.

- **Categories:** show their description.
- **Components:** show their cost.
- **Blueprints:** list how many components they have. 

Tap ![Edit](assets/edit.svg) to edit a row. You don't have to tap the icon directly. Tapping anywhere on the card 
does the same thing. ![Delete](assets/delete.svg) deletes that one record after a confirmation.

> **Note:** On the Blueprints list the caption says something like `3 components`. This is not the quantity of 
> components in the blueprint, but the number of individual components and other blueprints that have been linked 
> with that specific blueprint. 
> 
> For example:  
> Suppose you have a blueprint for Iron Ingot that takes 5 Iron Ore and 10 Wood. You have a second blueprint for 
> Iron Axe that takes 2 Iron Ingot and 5 Wood. On the Blueprint list, both the Iron Axe and Iron Ingot blueprints 
> will show the caption `2 components`.

### Search and filter

The ![Search](assets/search.svg) bar at the top filters as you type, matching anywhere in the name. So
`iron` finds `Wrought Iron`, `Iron Ore`, and `Iron Axe`. The **×** clears it.

Next to it, the ![Filter](assets/filter-list.svg) icon narrows by category. See
[Filtering by category](categories.md#filtering-by-category).

### The actions

These [actions](actions-bar.md) show up whenever you go to one of the lists for Categories, Components, or Blueprints.

| Action                                                  | What it does                                |
|---------------------------------------------------------|---------------------------------------------|
| ![New](assets/add.svg) **New**                          | Opens an empty editor for this record type. |
| ![Duplicate](assets/content-copy.svg) **Duplicate**     | Turns on Duplicate mode.                    |
| ![Delete](assets/delete.svg) **Delete**                 | Turns on Delete mode.                       |
| ![Delete all](assets/delete-forever.svg) **Delete all** | Wipes this list after a confirmation.       |

The last three gray out when the list is empty, since there would be nothing to act on.

### Duplicate mode

Tap ![Duplicate](assets/content-copy.svg) **Duplicate**, then tap any row. The editor opens on a
**copy** of that record, named `Whatever - Copy`, with nothing saved yet. Change what you need, fix the
name, tap **Save**.

This is the fast way to build out an armor set or a tool tier, where five recipes differ by one
material. Tap ![Duplicate](assets/content-copy.svg) again to leave the mode without doing anything.

### Delete mode (deleting several at once)

Tap ![Delete](assets/delete.svg) **Delete**, then tap every row you want gone. Selected rows will highlight. Then 
tap it a second time to delete all the selected records. You get a confirmation naming how many records are about 
to go.

> **Tip:** Changed your mind? **Press and hold the ![Delete](assets/delete.svg) action** to leave the mode with
> nothing deleted and your selection discarded.

## Editing a record

Tapping a row opens its editor. The fields differ by type. See [Blueprints](blueprints.md),
[Components](components.md) and [Categories](categories.md) for details about each type. The bar at the bottom is 
always the same:

- ![Delete](assets/delete.svg) **Delete:** (on the left, and only on a record that already exists) removes it, after 
  asking.
- **Cancel:** goes back without saving.
- ![Save](assets/save.svg) **Save:** commits. It stays grayed out until the record has a name, because
  a name is required for each record.

**Your edits are safe from a stray tap.** If you have unsaved changes and you hit Cancel, the back
arrow, or a navigation button, the app stops and asks whether to discard them.

## What deleting actually does

Records reference each other, so it is worth knowing what a delete ripples into:

| You delete…     | …and this happens                                                                         |
|-----------------|-------------------------------------------------------------------------------------------|
| A **category**  | Its components and blueprints survive, and become uncategorized.                          |
| A **component** | It's removed from every blueprint that used it. Those blueprints survive.                 |
| A **blueprint** | It's removed from every blueprint that uses it, and from every favorite that included it. |

Deletes are **permanent** and cannot be undone. This is exactly why every one of them asks you to confirm first. No one 
wants to accidentally delete something they spent the past couple of minutes creating.

## Delete all data

You can Delete All Data with the ![Delete all data](assets/delete-forever.svg) action on the Dataset landing page. 
This is the sledgehammer that you probably won't use very often, if even at all. But it exists to give you the 
option to clear everything and start over. It removes **every** favorite, blueprint, category and component in the 
dataset you're currently in. Your other [datasets](managing-datasets.md) aren't touched, and the dataset itself 
survives; it's just empty afterwards.

Because deletes are permanent, it doesn't settle for a yes/no box: you have to **type the word
DELETE** to confirm. If a batch was in progress on the Craft screen, it is cleared too, since it would
otherwise be pricing out blueprints that no longer exist.

Use it if your dataset is no longer useful, and you want a clean slate. Don't use it because a list 
looks cluttered. **Delete Mode**, inside the record lists, is the smaller hammer to clean up things you don't need 
anymore. And if you want the dataset gone entirely rather than just emptied, that's
[Delete dataset](managing-datasets.md#deleting-one).
