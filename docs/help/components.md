---
title: Components
nav_order: 5
---

# Components

Components are the stuff at the bottom. The raw materials. The things you gather rather than craft.

Wood. Stone. Iron Ore. Fiber. Leather. Sulfur. Star Metal. Flint. Feathers. Whatever your game makes
you go out and collect.

The rule of thumb: **if you get it by mining, chopping, farming, looting, killing or buying it, it is a
component. If you get it by standing at a bench, it is a [blueprint](blueprints.md).**

Components never break down further. When the app flattens a recipe tree, components are where it
stops — which is exactly why the **Components** tab on the Craft screen is a usable shopping list.

## Creating one

**Dataset → Components → ![New](assets/add.svg)**, or tap an existing component's row to edit it.

### Name

What the game calls it. Match the spelling; you will be searching for it later.

### Cost per item

What **one** of them costs you. This is what drives the **Cost** and **Profit** lines on the Craft
screen.

You have options here, and none of them are wrong:

- **Actual currency**, if your game has a shop or a trader.
- **An effort score.** Wood is 1, Iron Ore is 5, Black Metal Scrap is 40. Now "Cost" reads as "how much
  of a slog is this project", and Profit tells you whether the payoff is worth the grind.
- **Zero.** If you only want the shopping list, leave every cost at 0 and simply ignore the money lines.

### Production time per item

How long **one** takes to gather or produce, in hours, minutes and seconds.

Raw materials can absolutely cost time — a potato has to be grown before it can be cooked, iron has to
sit in a smelter, and a Rust furnace does not smelt instantly. If that matters to your planning, put it
here and the batch's **Crafting Time** will account for it.

Leave it at zero and it reads as *Instant*, which is the honest answer for something you pick up off
the ground.

### Category

Optional label. See [Categories](categories.md).

### Description

Free text — where it drops, which biome, which tool tier you need to harvest it, that sort of thing. It
shows on the detail card whenever you tap the component anywhere in the app.

## Should this be a component or a blueprint?

The honest answer: **whichever saves you work.**

The app does not care. If the intermediate step is not interesting to you, model it as a component and
give it a cost that already accounts for the hassle.

> **Example.** In Valheim, Bronze is a real recipe: 2 Copper + 1 Tin. Model it as a blueprint and the
> app will chase your axes all the way back to raw ore.
>
> But if you only ever buy Bronze from a chest full of it, model Bronze as a *component* with a cost,
> and stop there. Nobody is grading you.

The one thing you gain by modelling it as a blueprint is that the app counts the crafting step, tracks
the production time, applies the yield rounding, and reports the surplus. If none of that matters for a
given item, a component is less work.

## Saving, duplicating and deleting

Same as everywhere else: ![Save](assets/save.svg) **Save** is greyed out until there is a name,
**Cancel** and the back arrow warn you about unsaved edits, and ![Delete](assets/delete.svg) **Delete**
asks before it acts.

Deleting a component that a blueprint uses removes it from that blueprint's part list. The blueprint
itself survives — it just needs one fewer thing.

To create a near-copy, use ![Duplicate](assets/content-copy.svg) **Duplicate** on the Components list.
See [The Dataset Screen](dataset.md).
