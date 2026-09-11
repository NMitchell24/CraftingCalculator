---
title: Getting Started
nav_order: 2
---

# Getting Started

If you just installed the app and opened it for the first time, it will be completely empty. That's on purpose: 
there are a *lot* of survival crafting games, and none of them agree on what an "ingot" is. The **Dataset** is 
yours to build.

The good news is that the fastest useful setup only takes a few minutes, and you only ever have to do
it once per game.

We will build a small **Valheim** example to show how the app works, all the way from nothing to a finished number. 
Swap in your own game if you'd like. The steps you need to take are identical.

## Step 1: Make a few categories (optional, but future you will appreciate it)

Categories are just labels. You don't *really* need them, but if you add them, they'll make it a lot easier to 
find things once you've built out several records. So let's start by adding a couple categories. The basic steps 
look like this.

1. Tap ![Dataset](assets/menu-book.svg) **Dataset** in the navigation bar.
2. Tap the ![Categories](assets/label.svg) **Categories** card.
3. Tap the ![New](assets/add.svg) action.
4. Type in a name like `Ores`.
5. Fill in a description if you'd like.
6. Tap **Save**.

Make two or three: `Ores`, `Wood`, `Gear`. That's plenty to start.

> **Tip:** Adding multiple items with near identical values? Duplicate 'em!\
> **Duplicate** mode (![Duplicate](assets/content-copy.svg)) opens the editor pre-filled from the row you tap with 
> '- Copy' added to the name. So, duplicate, change the name, save, and do it again. It's that easy!
>
> You can duplicate Categories, Components, or Blueprints. See [Duplicate mode](dataset.md#duplicate-mode) for more 
> info.

More detail: [Categories](categories.md).

## Step 2: Add your raw components

Components are the things you *get*, not the things you *make*. If you swing an axe at it, shoot it,
pick it up off the floor, or buy it from a trader, it's very likely the thing is a component. Think of these as the raw 
materials that everything else is built from. Sometimes these things can be crafted, but if you never craft them, 
just add them as Components. Let's add a few of those too.

1. ![Dataset](assets/menu-book.svg) **Dataset** → ![Components](assets/inventory-2.svg) **Components** → ![New](assets/add.svg) **Add**
2. **Name**: `Tin Ore`
3. **Cost per item**: what one costs you. Use real in-game cost if your game has one, or leave it at 0 if you don't 
   care about money. But, who doesn't care about money, amiright?
4. **Production time per item**: how long one takes to gather or smelt. Optional. Leave it at zero and
   it reads as *Instant*. Or, just guestimate it. This will allow you to calculate how long it would take you to grind out the raw materials you need.
5. **Category**: `Ores`
6. **Save**.

Repeat for the handful of raw materials your recipe needs. For our example you will need: `Tin Ore`, `Copper Ore`,
`Wood`. Make sure you use the applicable category for each of these elements.

More detail: [Components](components.md).

## Step 3: Add a blueprint that uses them

Now we get into the real reason you're here. It's time to create your first **Blueprint**.

1. ![Dataset](assets/menu-book.svg) **Dataset** → ![Blueprints](assets/handyman.svg) **Blueprints** → ![New](assets/add.svg) **Add**
2. **Name**: `Bronze`
3. **Value per item**: what one finished Bronze is worth to you or your in-game economy. Again, this is entirely optional.
4. **Yield per craft**: how many the recipe produces in one go. A Valheim forge hands you **1** Bronze per craft, 
   so leave it at 1.  
5. **Production time per craft**: how long one craft takes at the bench in hours, minutes, and/or seconds. Optional. 
6. Tap or click on the **Add component** panel to open it.
7. Leave the toggle on **Component**, search for `Copper Ore`, set the quantity to `2`, tap **Add**.
8. Search for `Tin Ore`, quantity `1`, tap **Add**.
9. **Save**.
10. That's it. Your first **Blueprint** is locked in. But hold on tight, we're not quite finished yet.

More detail: [Blueprints](blueprints.md).

> **Tip:** The **Yield per craft** and **Production time per craft** fields can matter enormously. As a very basic 
> example, in Rust the Wooden Arrow recipe yields 2 ammo in 5 seconds. If you fill in that data here, it will allow 
> the app to tell you that crafting a full stack of arrows (64) will require queueing 32 crafts, take 2 minutes 
> and 40 seconds, and that you will need 750 Wood and 320 Stone. You could create the full stack of arrows as 
> a **Blueprint** by itself with that information! Then the app can tell you the total materials needed and how 
> long it will take if you want to craft 5, 10, 20 or even 100+ stacks. *I know... **no one** needs that many arrows... 
> unless they're playing a modded prim server... but I digress, it's just a basic example.*
> 
> For more info see [How the Math Works](calculations.md).

## Step 4: Nest a blueprint inside another blueprint

This is the thing that makes the app worth having, takes all that math out of your spreadsheets, and puts it in the 
palm of your hand.

1. ![Dataset](assets/menu-book.svg) **Dataset** → ![Blueprints](assets/handyman.svg) **Blueprints** → ![New](assets/add.svg) **Add**
2. **Name**: `Bronze Axe`
3. Open **Add component**, add `Wood` × 2.
4. Now flip the toggle from **Component** to **Blueprint**, search for `Bronze`,
   quantity `4`, tap **Add**.
5. **Save**.

You have now told the app that a Bronze Axe needs 4 Bronze and 2 wood. The **Blueprint** for a Bronze sword. Now, 
because the app already knows how  o make Bronze, it therefore knows that you need 8 Copper Ore and 4 Tin Ore on 
top of the 2 Wood. And it also knows that crafting your bronze axe is a 3-step process. 2 crafting steps for Bronze,
and then a final step for the Axe itself.

## Step 5: Craft something

1. Tap ![Craft](assets/calculate.svg) **Craft** in the navigation bar.
2. Tap **Craft Blueprint**, or the ![Add](assets/add.svg) action.
3. Tick `Bronze Axe` and confirm.
4. Use the ![Plus](assets/add.svg) stepper to set the quantity to 3.

Look at the **Components** list. There is your farming trip (or shopping list if you can't be bothered to actually 
mine anything): 24 Copper Ore, 12 Tin Ore, 6 Wood. Look at the **Crafting Summary** for the totals, and the 
**Steps** tab for the whole tree, in order.

If you filled in the Production Time and Cost/Value fields the app can tell you how long it will take and how 
much you stand to profit if you make 10, 20, or even 100 Bronze Axes. You can kiss that pesky spreadsheet goodbye.

More detail: [The Craft Screen](craft-screen.md).

## Step 6: Save it for next time

Have a blueprint or set of blueprints that you need to refer back to frequently? Tap ![Save as favorite](assets/save.svg)
**Save as favorite**, give it a name like `Money Maker`, or `Gear Kit`, and it's right at your fingertips forever.

More detail: [Favorites](favorites.md).

## The order that always works

If you ever get stuck, remember the order things are listed in this guide. It is the same order the Dataset screen lists
them in, top to bottom:

**Categories** → **Components** → **Blueprints** → **Craft**

You can't put a component into a blueprint before you've created the component, and categories can be linked with 
both components and blueprints. Build from the bottom of the tree upward and you'll never hit a dead end.
