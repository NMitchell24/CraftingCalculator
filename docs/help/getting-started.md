---
title: Getting Started
nav_order: 2
---

# Getting Started

A brand new install is completely empty. That is on purpose: there are a *lot* of survival crafting
games, and none of them agree on what a "plank" is. The dataset is yours to build.

The good news is that the fastest useful setup takes about five minutes, and you only ever have to do
it once per game.

We will build a tiny **Valheim** example, all the way from nothing to a finished number. Swap in your
own game as you go — the shape is identical.

## Step 1 — Make a few categories (optional, but future you says thanks)

Categories are just labels. Skip them and nothing breaks. Add them and, three hundred records later,
you can still find the one you want.

1. Tap **Dataset** in the navigation bar.
2. Tap the **Categories** card.
3. Tap the ![New](assets/add.svg) action (bottom bar on a phone, side drawer on a tablet or desktop).
4. Type a name — say `Ores` — and tap **Save**.

Make two or three: `Ores`, `Wood`, `Gear`. That is plenty to start.

More detail: [Categories](categories.md).

## Step 2 — Add your raw components

Components are the things you *get*, not the things you *make*. If you swing a pickaxe at it, shoot it,
pick it up off the floor or buy it from a trader, it is a component.

1. **Dataset → Components → ![New](assets/add.svg)**
2. **Name**: `Tin Ore`
3. **Cost per item**: what one costs you. Use real in-game currency if your game has one, or use it as
   a "how annoying is this to get" score — 1 for a twig, 50 for a boss drop. Leave it at 0 if you do
   not care about money.
4. **Production time per item**: how long one takes to gather or smelt. Optional. Leave it at zero and
   it reads as *Instant*.
5. **Category**: `Ores`
6. **Save**.

Repeat for the handful of raw materials your recipe needs. For our example: `Tin Ore`, `Copper Ore`,
`Wood`.

More detail: [Components](components.md).

## Step 3 — Add a blueprint that uses them

Now the fun part.

1. **Dataset → Blueprints → ![New](assets/add.svg)**
2. **Name**: `Bronze`
3. **Value per item**: what one finished Bronze is worth to you. Again, optional.
4. **Yield per craft**: how many the recipe produces in one go. A Valheim forge hands you **1** Bronze
   per craft, so leave it at 1. This field matters enormously — the Minecraft planks recipe yields 4.
   See [How the Math Works](calculations.md).
5. **Production time per craft**: how long one craft takes at the bench. Optional.
6. Open the **Add component** panel.
7. Leave the toggle on **Component**, search for `Copper Ore`, set the quantity to `2`, tap **Add**.
8. Search for `Tin Ore`, quantity `1`, tap **Add**.
9. **Save**.

More detail: [Blueprints](blueprints.md).

## Step 4 — Nest a blueprint inside another blueprint

This is the thing that makes the app worth having.

1. **Dataset → Blueprints → ![New](assets/add.svg)**
2. **Name**: `Bronze Axe`
3. Open **Add component**, flip the toggle from **Component** to **Blueprint**, search for `Bronze`,
   quantity `4`, tap **Add**.
4. Flip the toggle back to **Component**, add `Wood` × 2.
5. **Save**.

You have now told the app that a Bronze Axe needs 4 Bronze, and — because it already knows the Bronze
recipe — that it therefore needs 8 Copper Ore and 4 Tin Ore. You never have to do that multiplication
again.

## Step 5 — Craft something

1. Tap **Craft** in the navigation bar.
2. Tap **Craft Blueprint**, or the ![Add](assets/add.svg) action.
3. Tick `Bronze Axe` and confirm.
4. Use the ![Plus](assets/add.svg) stepper to set the quantity to 3.

Look at the **Components** list. There is your shopping trip: 24 Copper Ore, 12 Tin Ore, 6 Wood. Look
at the **Crafting Summary** for the totals, and the **Steps** tab for the whole tree, in order.

More detail: [The Craft Screen](craft-screen.md).

## Step 6 — Save it for next time

Building the same set of gear on every new server? Tap ![Save as favorite](assets/save.svg)
**Save as favorite**, give it a name like `Bronze Age Starter Kit`, and it is one tap away forever.

More detail: [Favorites](favorites.md).

## The order that always works

If you ever get stuck, remember the dependency order. It is the same order the Dataset screen lists
them in, top to bottom:

**Categories** → **Components** → **Blueprints** → **Craft**

You cannot put a component into a blueprint before you have created the component. Build from the
bottom of the tree upward and you will never hit a dead end.
