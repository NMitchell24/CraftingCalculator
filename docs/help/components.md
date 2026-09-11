---
title: Components
nav_order: 6
---

# Components

Components are the raw materials. The things you gather, purchase, steal, or can't be bothered to craft.

Games have lots of names for this stuff: Wood, Stone, Iron Ore, Fiber, Leather, Sulfur, Carbon, Frost Crystals, 
Flint, Chitin, Adhesive, Ceramic, I could keep going, but I think you get the point. Whatever your game makes you go 
out and collect before you can craft something is a component.

The rule of thumb: **if you get it by mining, chopping, farming, looting, killing or buying it — and it can't be 
crafted by itself — it's a component. If you can craft it, it's a [blueprint](blueprints.md).**

Components are the last leaf on every branch in the tree. When the app flattens a recipe tree, components are where it
stops. The **Components** tab on the [Craft screen](craft-screen.md) is what you need to gather or purchase before 
you can craft.

## Creating one

**![Dataset](assets/menu-book.svg) Dataset → ![Components](assets/inventory-2.svg) Components → ![New](assets/add.svg) Add**, or tap an existing component's row to edit it.

### Name

Whatever you want. I'm not your mom. I won't tell you what to do. But I'd strongly suggest calling it whatever the 
game calls it. This is what you will use when you're searching for it later both in the app and in your game. So it 
should make sense. 

### Cost per item

What **one** of them costs you. This is what drives the **Cost** and **Profit** lines on the Craft
screen.

You have options here, and none of them are wrong:

- **Actual currency**, if your game has a shop or a trader.
- **An effort score.** Wood is 1, Iron Ore is 5, Black Metal Scrap is 40. Now "Cost" reads as "how much
  of a slog is this project", and Profit tells you whether the payoff is worth the grind.
- **Zero.** If you only want the raw materials, leave every cost at 0 and simply ignore the money lines.

### Production time per item

How long **one** takes to gather or produce, in hours, minutes and seconds.

Raw materials can absolutely cost time — a potato has to be grown before it can be cooked, ore has to smelt, and a Rust 
furnace doesn't smelt instantly. If that matters to your planning, put it here and the batch's **Crafting Time** 
will account for it.

Leave it at zero and it reads as *Instant*, which is the honest answer for something you pick up off
the ground.

The numbers don't have to be exact. Even a rough estimate can give you a better idea of how long it will take to 
collect 1000 of these than just leaving it at 0.

### Category

Optional label. See [Categories](categories.md).

### Description

Free text. You can put in details like where it drops, which biome, which tool tier you need to harvest it, that 
sort of thing. It shows on the detail card whenever you tap the component anywhere in the app.

## Should this be a component or a blueprint?

The honest answer: **whichever saves you work.**

The app doesn't care. If the intermediate step isn't interesting to you, model it as a component. If you hate crafting 
the thing, and it's easy to buy, create it as a component and give it a cost so you can manage your profit margins.

> **Example:** In Rust, Charcoal is *technically* a recipe. You stick wood in a campfire, 
> furnace, refinery, whatever, and it burns, producing Charcoal 1:1 on a tick rate that varies based on what you 
> use. You could put that in as a blueprint. But, why? You're already going to get Charcoal through your other 
> activities. So list it as a component. Then your Gunpowder recipe stops at the Charcoal and doesn't tell you 
> that you need a bunch of wood to make charcoal to make GP.

The one thing you gain by modeling it as a blueprint is that the app counts the crafting steps, tracks
the production time, applies the yield rounding, and reports the surplus. If none of that matters for a
given item, a component is less work.

## Saving, duplicating and deleting

Same as everywhere else: ![Save](assets/save.svg) **Save** is grayed out until there is a name,
**Cancel** and the back arrow warn you about unsaved edits, and ![Delete](assets/delete.svg) **Delete**
asks before it acts.

Deleting a component that a blueprint uses removes it from that blueprint's part list. The blueprint
itself survives. It just needs one less thing.

To create a copy of one, use ![Duplicate](assets/content-copy.svg) **Duplicate** on the Components list.
See [The Dataset Screen](dataset.md) for more info.
