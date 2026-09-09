---
title: Blueprints
nav_order: 4
---

# Blueprints

A blueprint is a recipe. It is the thing you actually want: a Bronze Axe, a Longship, a Hazmat Suit, a
stack of Cooked Meat, an entire base wall section.

Every blueprint is a name plus a list of parts, and each part is either a [component](components.md) —
raw stuff — or **another blueprint**. That second option is the whole trick. Describe each recipe
exactly once, at the level the game states it, and let the app chase the dependencies down for you.

## Creating one

**Dataset → Blueprints → ![New](assets/add.svg)**, or open an existing blueprint by tapping its row.

### Name

What the game calls it. This is what shows up in the picker, the batch, the tree and your copied
shopping lists, so match the game's spelling — future you will be searching for it.

### Value per item

What **one finished item** is worth. This feeds the **Value** and **Profit** lines on the Craft screen.

Use whatever makes sense for your game:

- **Rust** — scrap value, or what it actually sells for at a vending machine.
- **Conan Exiles** — gold, or what a thrall-run shop pays.
- **Minecraft** — emeralds, or just a made-up "how good is this" score.
- **Valheim** — coins, if you plan on visiting Haldor.

Leave it at 0 if you only care about materials. The Value and Profit lines will simply read zero and
you can ignore them.

### Yield per craft

**How many items one craft produces.** The minimum is 1, and it is 1 by default.

This is the single most commonly missed field, and getting it wrong quietly doubles your shopping list.

| Game | Recipe | Yield |
|---|---|---|
| Minecraft | 1 log → planks | **4** |
| Minecraft | 3 wheat → bread | **1** |
| Rust | 1 cloth → rope | **1** |
| Valheim | 1 wood → coal in a kiln | **1** |
| Conan Exiles | 1 branch → 5 firewood | **5** |

If your game hands you a stack when you pull the lever, the size of that stack goes here. The app then
works out how many times you have to pull the lever, rounds up — because you cannot craft two thirds of
a recipe — and reports the leftovers as [surplus](calculations.md#surplus).

### Production time per craft

How long **one craft** takes, in hours, minutes and seconds. Seconds accept a decimal, so a 1.5-second
smelt is expressible.

Leave it all at zero for instant crafts. The field shows you a live preview of what you typed —
`Instant`, `5.5s`, `4:30`, `2:04:30`, or `3 Days 02:04:30` — so you can check it before saving.

This is per *craft*, not per *item*. A recipe that yields 4 in 10 seconds has a production time of
10 seconds, not 2.5.

### Category

Optional label. See [Categories](categories.md).

### Description

Free text. Notes to yourself: which workbench it needs, which tier it unlocks at, whether it needs a
Level 3 forge. It shows up on the detail card when you tap the blueprint anywhere in the app.

## Adding parts

Open the **Add component** panel and you get three controls:

1. **A Component / Blueprint toggle.** This decides which of your records the search box hunts through.
2. **A search box.** Start typing; it filters as you go.
3. **A quantity, and the ![Add](assets/add.svg) Add button.**

Add the same part twice and the quantities merge rather than creating a duplicate row.

Underneath, each part you have added gets a row with ![Minus](assets/remove.svg), an editable number,
![Plus](assets/add.svg), and ![Delete](assets/delete.svg). Setting a quantity to 0 removes the part
completely.

### The nesting rule

Point a blueprint at another blueprint whenever the game makes you craft an intermediate item.

> Longship needs **Iron Nails**. Iron Nails are their own recipe made from Iron. So `Longship` contains
> the *blueprint* `Iron Nails`, and `Iron Nails` contains the *component* `Iron`.

Do **not** flatten it yourself. If you write the Longship as "100 Iron", you lose the crafting steps,
the time, and the yield rounding — and the day the game rebalances the nail recipe you have to redo the
arithmetic by hand.

Nest as deep as your game does. Modded Minecraft packs go a long way down and the app follows.

### One thing it will not let you do

A blueprint cannot contain **itself**. If a chain of blueprints ever loops back around on itself, the
app stops with an error naming the blueprint rather than spinning forever. If you see that message, look
for a recipe you accidentally pointed back at one of its own ancestors.

## Saving, duplicating and deleting

- ![Save](assets/save.svg) **Save** commits your changes. It stays greyed out until the blueprint has a
  name.
- **Cancel** or the back arrow abandons them — and if you have unsaved edits, you get asked first.
- ![Delete](assets/delete.svg) **Delete**, at the bottom left, removes the blueprint after a
  confirmation.

To make a near-copy — armour sets and tool tiers are usually 90% identical — use
![Duplicate](assets/content-copy.svg) **Duplicate** on the Blueprints list. See
[The Dataset Screen](dataset.md).

## Deleting a blueprint that is used elsewhere

Deleting a blueprint that another blueprint nests removes it from that parent's part list too. The
parent survives; it just gets shorter. Same for a favorite: deleting a blueprint drops it out of any
saved batch that referenced it.
