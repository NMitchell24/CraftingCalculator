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

**To add something**, tap **Craft blueprint** in the middle of the empty card, or the ![Add](assets/add.svg) action. 
A picker opens with every blueprint in your Dataset. Search it, 
[filter it by category](categories.md#filtering-by-category), and tap ![Add](assets/add.svg) **Add** on anything you
want to build. It drops straight into your batch with a quantity of 1, and its card swaps **Add** for the same
stepper the batch uses: ![Minus ten](assets/minus-10.svg) ![Minus one](assets/minus-1.svg), a number you can edit, and
![Plus one](assets/plus-1.svg) ![Plus ten](assets/plus-10.svg). So you can set the quantity without leaving the picker.
Changed your mind? ![Delete](assets/delete.svg) takes it back out of the batch and the card goes back to **Add**. Every
card also has ![Info](assets/info.svg), for when you can't remember which of your three "Bronze Axe (the good one)"
blueprints is actually the good one. There's no confirm step. Tap ![Close](assets/close.svg) **Close** in the top
corner when you're done, or use your phone's back gesture.

**Each card in the batch gives you:**

| Control                                                                                                                           | What it does                                                                                           |
|-----------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------|
| ![Minus ten](assets/minus-10.svg) ![Minus one](assets/minus-1.svg) ![Plus one](assets/plus-1.svg) ![Plus ten](assets/plus-10.svg) | Step the quantity down or up by ten or by one. See [Stepping by ten](craft-screen.md#stepping-by-ten). |
| **The number**                                                                                                                    | Tap it and type a quantity directly. Handy when you need 240 of something.                             |
| ![Info](assets/info.svg)                                                                                                          | Opens a detail card showing the value, yield, production time, and the recipe's own requirements.      |
| ![Delete](assets/delete.svg)                                                                                                      | Removes that blueprint from the batch.                                                                 |

Stepping down **stops at zero**, and stepping down again **from zero removes the row**. Zero itself is a
perfectly valid quantity. It keeps the blueprint in your batch while contributing nothing, which is a quick way to
ask "what if I skipped this one?" without losing your place.

**Load Favorite** at the top of the card swaps your whole batch for a saved one. See
[Favorites](favorites.md).

**The batch keeps up with your edits.** Say you've got Bronze in the batch and then install a mod that makes it
cost 3 Copper instead of 2. Fix the blueprint on the [Dataset screen](dataset.md), come back, and the batch is
already asking for 3. Same goes for a new cost on Copper or a renamed category. Your quantities stay exactly where
you left them. Delete a blueprint that's in the batch, and it drops out of the batch too, since there's nothing left
to craft.

### Stepping by ten

Tapping ![Plus one](assets/plus-1.svg) sixty times to get to 60 arrows is the exact kind of grind this app is
supposed to save you from. So every card has a second pair of buttons on the outside of the first.
![Minus ten](assets/minus-10.svg) takes ten off and ![Plus ten](assets/plus-10.svg) adds ten. That's it. Same buttons
on a phone, a tablet or a PC, and no modes to remember.

You can still tap the number and type a quantity straight in.

> **Note:** stepping down by ten stops at zero. A row sitting at 4 lands on zero rather than -6, so one
> mistimed tap can't quietly drop a blueprint out of your batch. Tap down once more while it's sitting
> at zero and the row goes away, exactly like it does when you're stepping by one.

## Crafting Summary

Up to eight numbers, all of them linked directly to your selected blueprint(s). They update the instant you change a 
blueprint quantity. Some of them go away when a [Datasetting](dataset.md#datasettings) is off:

- **[Calculate Yield](dataset.md#calculate-yield) off:** no Surplus Stock or Surplus Value.
- **[Calculate Costs](dataset.md#calculate-costs) off:** no Cost.
- **[Calculate Values](dataset.md#calculate-values) off:** no Value or Surplus Value. If Calculate Costs is still on,
  Profit is renamed **Cost** and shows what the batch costs as a negative number, in place of the usual Cost line.
- **Calculate Costs and Calculate Values both off:** no Profit either.

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

Each card shows the component's name and two lines of figures: **Quantity** (`Quantity: x48`) and the
**Cost** of that many. With [Calculate Costs](dataset.md#calculate-costs) off, it's just the quantity.

> **Tip:** Tap ![Info](assets/info.svg) on a card to show a detailed breakdown of that component with its Name,
> Description, Category and cost. 
> 
> Or tap ![Copy](assets/content-copy.svg) **Copy** to put the whole list on your clipboard, ready to paste into a 
> Discord message, a note, or a clan roster.

### Steps

The full breakdown, as a tree. It traces down from the item you requested to the raw materials required for every step 
along the way. The top row is a blueprint from your batch. Expand that row and you can see the requirements it 
needs. Expand those rows to see the requirements **they** need, and so on, all the way down to raw components.

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

> **Note:** This tab only shows up when [Calculate Yield](dataset.md#calculate-yield) is on. With it off, nothing is
> ever overproduced, so there'd be nothing to list.

The leftovers. The extra crap you can save for later. When a recipe yields 4 and you need 6, you have to run it 
twice. When you do, you end up with 2 more than you needed. This tab lists every one of those spares across the 
whole batch along with what they are worth.

Each card works like the Components tab's: the blueprint's name, **Quantity** (`Quantity: x2`), and the
**Value** of those spares. With [Calculate Values](dataset.md#calculate-values) off, it's just the quantity.
![Info](assets/info.svg) opens the blueprint's detail card.

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
