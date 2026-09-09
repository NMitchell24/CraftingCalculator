---
title: The Craft Screen
nav_order: 3
---

# The Craft Screen

This is the reason the app exists. Everything else is set-up; this is where you get your answer.

The screen has two halves. On a phone they stack, and on a tablet or a desktop window they sit side by
side, but they are always the same two halves:

- **The batch** — what you want to build, and how many.
- **The results** — what that will cost you.

## Selected Blueprints

Your batch. The list of things you are asking for.

**To add something**, tap **Craft Blueprint** in the middle of the empty card, or the
![Add](assets/add.svg) action. A picker opens with every blueprint you have. Search it, filter it by
category, tick as many as you like, then confirm — they all drop into the batch at a quantity of 1.

**Each row in the batch gives you:**

| Control | What it does |
|---|---|
| ![Minus](assets/remove.svg) ![Plus](assets/add.svg) | Step the quantity down or up by one. |
| The number itself | Tap it and type a quantity directly. Handy when you need 240 of something. |
| ![Info](assets/info.svg) | Opens a detail card: value, yield, production time, and the recipe's own parts. |
| ![Delete](assets/delete.svg) | Removes that blueprint from the batch. |

Stepping the quantity **below zero removes the row**. Zero itself is a perfectly valid quantity — it
keeps the blueprint in your batch while contributing nothing, which is a quick way to ask "what if I
skipped this one?" without losing your place.

**Load Favorite** at the top of the card swaps your whole batch for a saved one. See
[Favorites](favorites.md).

## Crafting Summary

The scoreboard. Eight numbers, all of them live — they update the instant you change a quantity.

| Line | What it is telling you |
|---|---|
| **Components Needed** | Total raw items to gather. Every component in the batch, added up. |
| **Crafting Steps** | How many individual crafts you will perform, at every level of every tree. |
| **Crafting Time** | How long all of that takes, if you did it one step at a time. |
| **Cost** | What the raw components cost, using the Cost you gave each one. |
| **Value** | What the finished items are worth, using the Value you gave each blueprint. |
| **Surplus Stock** | Extra items the crafts produce beyond what you asked for. |
| **Surplus Value** | What that extra is worth. |
| **Profit** | Value minus Cost. Green if you are up, red if you are down. |

Surplus is deliberately *not* folded into Profit. It is stock sitting in your chest, not a sale. See
[How the Math Works](calculations.md) for the full reasoning and worked examples.

## The three result tabs

The other card switches between three views of the same batch.

### Components

Your shopping list: every raw component the entire batch needs, merged together and sorted by name,
with the quantity and total cost of each. If three different blueprints in your batch each want Iron,
you get **one** Iron line with the combined number — which is exactly what you want when you are
standing in a mine.

Tap any row for a detail card. Tap ![Copy](assets/content-copy.svg) **Copy** to put the whole list on
your clipboard, ready to paste into a Discord message, a note, or a clan roster.

### Steps

The full breakdown, as a tree, in dependency order. The top of each branch is a blueprint from your
batch; underneath are the parts it needs; underneath those are *their* parts, all the way down to raw
components.

- The **arrow** on the left expands and collapses a branch.
- **Tapping the row itself** opens its detail card — quantity, crafts, yield, surplus and time for that
  exact position in the tree.
- ![Expand all](assets/unfold-more.svg) **Expand all** and ![Collapse all](assets/unfold-less.svg)
  **Collapse all** do what they say.
- ![Blueprint](assets/handyman.svg) is a blueprint, something you craft.
  ![Component](assets/inventory-2.svg) is a component, something you gather.

Some rows carry an **asterisk**. That means the blueprint produces items in fixed batches, so the
number shown is *how many crafts you need*, not how many items you get. Tap the row to see the item
count. A legend explaining this appears under the tree whenever such a row is on screen.

### Surplus

The leftovers. When a recipe yields 4 and you need 6, you have to run it twice, and you end up with 2
spare. This tab lists every one of those spares across the whole batch, with what they are worth.

This is not an error — it is free stuff. It is the app telling you to check your chest before your next
project. Tap ![Copy](assets/content-copy.svg) **Copy** to take the list with you.

## The actions

On a phone, the page's actions live in the bar at the bottom of the screen. On a tablet or desktop
window they move into the side drawer. Same actions either way.

| Action | What it does |
|---|---|
| ![Add blueprints](assets/add.svg) **Add blueprints** | Opens the picker. |
| ![Clear selection](assets/clear.svg) **Clear selection** | Empties the batch. |
| ![Save as favorite](assets/save.svg) **Save as favorite** | Stores the current batch under a name. See [Favorites](favorites.md). |

## Nothing shows up when I tap ![Add](assets/add.svg)

Then you have not made any blueprints yet, and the app has nothing to offer you. Head to
[Getting Started](getting-started.md) — it takes five minutes.
