---
title: The Craft Screen
nav_order: 4
---

# The Craft Screen

This is the reason the app exists. Everything else is set up; this is where you get your answer.

The screen has two halves. On a phone they stack, and on a tablet or a desktop window they sit side by
side, but they are always the same two parts:

- **The batch:** what you want to build, and how many.
- **The results:** how much you need to grind, and what you stand to profit.

## Selected Blueprints

Your batch. AKA, the list of things you want to craft in your game.

**To add something**, tap **Craft Blueprint** in the middle of the empty card, or the ![Add](assets/add.svg) action. 
A picker opens with every blueprint in your Dataset. Search it, 
[filter it by category](categories.md#filtering-by-category), tick as many as you like, then confirm. Once finished,
everything you selected drops into your batch with a quantity of 1.

**Each row in the batch gives you:**

| Control                                             | What it does                                                                               |
|-----------------------------------------------------|--------------------------------------------------------------------------------------------|
| ![Minus](assets/remove.svg) ![Plus](assets/add.svg) | Step the quantity up or down by one.                                                       |
| **The number**                                      | Tap it and type a quantity directly. Handy when you need 240 of something.                 |
| ![Info](assets/info.svg)                            | Opens a detail card showing the value, yield, production time, and the recipe's own parts. |
| ![Delete](assets/delete.svg)                        | Removes that blueprint from the batch.                                                     |

Stepping the quantity **below zero removes the row**. Zero itself is a perfectly valid quantity. It
keeps the blueprint in your batch while contributing nothing, which is a quick way to ask "what if I
skipped this one?" without losing your place.

**Load Favorite** at the top of the card swaps your whole batch for a saved one. See
[Favorites](favorites.md).

## Crafting Summary

Eight numbers, all of them linked directly to your selected blueprint(s). They update the instant you change a 
blueprint quantity.

| Line                  | What it means                                                                         |
|-----------------------|---------------------------------------------------------------------------------------|
| **Components Needed** | Total raw items to gather. Every component in the batch, added up.                    |
| **Crafting Steps**    | How many individual crafts you will perform, at every branch of every tree.           |
| **Crafting Time**     | How long all of that takes, every step's total time summed up and formatted readably. |
| **Cost**              | What the raw components cost, using the Cost you gave each one.                       |
| **Value**             | What the finished items are worth, using the Value you gave each blueprint.           |
| **Surplus Stock**     | Leftover items the crafts produce, beyond what you asked for.                         |
| **Surplus Value**     | What all those leftovers are worth.                                                   |
| **Profit**            | Value minus Cost. Green if you're up, red if you're down.                             |

> **Note:** Surplus is deliberately *not* added into Profit. It is considered extra stock that you may want to use for 
> another craft. But the value of the surplus is listed separately so that you can always fold it into profit if 
> you'd like. 
> 
> See [How the Math Works](calculations.md) for the full reasoning and worked examples.

## The three result tabs

The other card switches between three views of the same blueprint batch.

### Components

Every raw component the entire batch needs, merged together and sorted by name,
with the quantity and total cost of each. If three different blueprints in your batch each want Iron,
you get **one** Iron line with the combined number. This is exactly what you want when you are
standing in a mine or purchasing ore from a trader.

> **Tip:** Tap any row to show a detailed breakdown of that component with its Name, Description, Category and cost. 
> 
> Or tap ![Copy](assets/content-copy.svg) **Copy** to put the whole list on your clipboard, ready to paste into a 
> Discord message, a note, or a clan roster.

### Steps

The full breakdown, as a tree. It traces down from the item you requested to the raw materials required for every step 
along the way. The top row is a blueprint from your batch. Expand that row and you can see the parts it 
needs. Expand those rows to see the parts **they** need, and so on, all the way down to raw components.

- The **arrow** on the left expands and collapses a branch.
- **Tapping the row itself** opens its detail card. This shows a breakdown of the quantity, crafts, yield, surplus 
  and time for that exact position in the tree.
- ![Expand all](assets/unfold-more.svg) **Expand all** and ![Collapse all](assets/unfold-less.svg)
  **Collapse all** do what they say.
- ![Blueprint](assets/handyman.svg) is a blueprint, something you craft.
  ![Component](assets/inventory-2.svg) is a component, something you gather or buy.

> **Note:** Some rows carry an **asterisk**. That means the blueprint produces items in fixed batches, so the number 
> shown is *how many crafts you need*, not how many items you get. Tap the row to see the item count. A legend 
> explaining this appears under the tree whenever such a row is on screen.

### Surplus

The leftovers. The extra crap you can save for later. When a recipe yields 4 and you need 6, you have to run it 
twice. When you do, you end up with 2 more than you needed. This tab lists every one of those spares across the 
whole batch along with what they are worth.

This isn't an error. It's just a side effect of a Blueprint that produces more than what you need. It's the app 
telling you that you get some extra junk you'll need to either store, sell, or use for another project. Tap 
![Copy](assets/content-copy.svg) **Copy** to save the list for later.

## The actions

These are the [actions](actions-bar.md) available from the craft screen. 

| Action                                                    | What it does                                                                                         |
|-----------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| ![Add blueprints](assets/add.svg) **Add blueprints**      | Opens the blueprint picker to add items into your batch.                                             |
| ![Clear selection](assets/clear.svg) **Clear selection**  | Clears all selected blueprints from the current batch.                                               |
| ![Save as favorite](assets/save.svg) **Save as favorite** | Stores the current batch under a name for you to easily recall later. See [Favorites](favorites.md). |

## Nothing shows up when I tap ![Add](assets/add.svg)

Then you haven't created any blueprints yet, and the app has no way of knowing what you want. Head to
[Getting Started](getting-started.md) to learn how to create your data. It only takes a few minutes.
