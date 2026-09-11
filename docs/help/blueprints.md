---
title: Blueprints
nav_order: 7
---

# Blueprints

A blueprint is a recipe. Each blueprint represents a **single unit** of the thing *you* want to make in the game. 
Blueprints are the things you craft: a Bronze Axe, a Stone Foundation, a Stasis Device, a Diamond Sword, Ammo, et 
cetera. Sometimes, it's even the thing you *have* to craft so that you can make the thing you *actually* want. For 
example, you can't craft Ammo without Gunpowder. Gunpowder is another thing you have to craft. So, both Ammo and 
Gunpowder are Blueprints. How much Ammo you can make just **depends** on how much Gunpowder you can make first.

Every blueprint is a name plus a list of parts. Each part is either a [component](components.md) or **another 
blueprint**. This last part is how you would link together the Ammo and Gunpowder blueprints from the example above. 
Gunpowder is a blueprint by itself that requires raw components of Sulfur and Charcoal. Ammo then requires 
Gunpowder and Metal. So the app now knows that to craft Ammo, you will need to collect Sulfur, Charcoal, and Metal. 
The Blueprint chain just allows the app to calculate how much of each resource you need to farm — and how much 
Gunpowder you need to make — to craft the quantity of Ammo you want.

## Creating one

![Dataset](assets/menu-book.svg) **Dataset** → ![Blueprints](assets/handyman.svg) **Blueprints** → ![New](assets/add.svg) **Add**, 
or open an existing blueprint by tapping its row.

### Name

This is what the game calls it, or really whatever you want to call it. This is what shows up in the picker, the batch, 
the tree and how you will search for it later. So make sure you call it something that you can remember easily.

### Value per item

What **one finished item** is worth if you were to sell it in the game. This feeds the **Value** and **Profit** 
lines on the Craft screen.

This can be whatever makes sense for your game: Caps, Tokens, Scrap, Units, Coins, Gold, whatever your game calls 
its currency.

Leave it at 0 if your game has no currency system or if you only care about materials. The Value and Profit lines 
will simply read zero, and you can ignore them.

### Yield per craft

**How many items one craft produces.** The minimum is 1, and it's 1 by default. In most cases, you can leave this at 
the default and ignore it. However, if you don't change it for a recipe that does yield more than 1 item, then you 
can unknowingly end up farming or crafting a lot more crap than what you need. So, pay attention to the recipe 
you're copying from the game, a wiki, or wherever. Crafting games can be grindy enough as it is. You don't need to 
add extra grind if you don't have to.

Here are some examples of games and recipes where this would come into play.

| Game         | Recipe                            | Yield |
|--------------|-----------------------------------|-------|
| Minecraft    | 1 log → planks                    | **4** |
| Rust         | 25 wood + 10 stone → wooden arrow | **2** |
| Conan Exiles | 1 branch → firewood               | **5** |

If your game hands you a stack of things every time you queue up a craft, the size of that stack goes here. The app then
works out how many times you have to craft the thing and rounds up. It always rounds up the crafting steps 
required because you can't craft a recipe halfway; the stack it produces is fixed. Anything extra that you 
don't need is reported as [surplus](calculations.md#surplus). After all, it's better to have a little bit extra than 
not enough.

> **Note:** Yield cannot be set to 0 or anything less than 0. A recipe that yields nothing *is* nothing and setting it 
> to 0 would cause the app to divide by zero and break the universe. Negative yields also don't make sense. There's 
> simply no case where crafting an item means you end up having to give back the materials you used.

### Production time per craft

How long **one craft** takes, in hours, minutes and/or seconds. Seconds accept a decimal, so a 1.5-second
smelt is perfectly fine. 

You can leave it all at zero if you don't care to track crafting time or if your game's craft time 
is instant. After you fill in any values, a live preview of what you typed displays below it. This will be formatted 
the same way it will show on the [Craft Screen](craft-screen.md) so you can check it before saving.

> **Note:** This is per *craft*, not per *item*. A recipe that yields 4 in 10 seconds has a production time of
> 10 seconds, not 2.5.

### Category

A custom label for the blueprint. These labels allow you to more easily filter your 
blueprints down when they display in lists. See [Categories](categories.md) for more information.

### Description

Free text. Notes to yourself: which workbench it needs, which tier it unlocks at, how to unlock it, whatever you want.
This shows up on the detail card when you tap the blueprint anywhere in the app.

## Adding parts

If you tap or click on the **Add component** panel it will expand with three controls:

1. **A Component / Blueprint toggle.** This defines which type of record you want to add.
2. **A search box.** Start typing; it filters as you go.
3. **A quantity and the ![Add](assets/add.svg) Add button.**

If you add the same part twice the quantities merge rather than creating a duplicate row.

Underneath, each part you've added gets a row with ![Minus](assets/remove.svg), an editable number,
![Plus](assets/add.svg), and ![Delete](assets/delete.svg). Setting a quantity to 0 removes the part
completely.

### Nesting blueprints

Point a blueprint at another blueprint whenever the game makes you craft an intermediate item.

> **Note:** Think back to our example at the top of this document. Ammo needs **Gunpowder**. Gunpowder is its own 
> blueprint made from Sulfur and Charcoal. So, `Ammo` contains the *blueprint* `Gunpowder` and the *component* 
> `Metal`. Then `Gunpowder` contains the *components* `Sulfur` and `Charcoal`.

**It's not recommended** to flatten it yourself. If you fill in Ammo as "10 Metal, 30 Charcoal, and 20 Sulfur", you 
lose the crafting steps, the time, and the yield rounding. The day the game rebalances the ammo recipe, you then have 
to redo all the math by hand rather than just editing the recipe(s) that changed.

Nest as deep as your game does. The app can follow the entire tree for a Stasis Device in No Man's Sky. I don't 
think you'll have any issues.

### One thing it won't let you do

A blueprint cannot contain **itself**. If a chain of blueprints ever loops back around on itself, the
app stops with an error naming the blueprint rather than spinning forever. If you see that message, look
for a recipe you accidentally pointed back at one of its own ancestors.

## What to do when you're done

- ![Save](assets/save.svg) **Save** commits your changes. It stays grayed out until the blueprint has a
  name.
- **Cancel** or the back arrow abandons them. If you have unsaved edits, you get asked first.
- ![Delete](assets/delete.svg) **Delete**, at the bottom left, removes the blueprint after a confirmation.

> **Tip:** Armor sets and tool tiers are usually 90% identical. To make a near-copy easily, you can use
> ![Duplicate](assets/content-copy.svg) **Duplicate** on the Blueprints list. 
> 
> See [The Dataset Screen](dataset.md) for more information.

## Deleting a blueprint that is used elsewhere

Deleting a blueprint that another blueprint nests removes it from that parent's list too. The parent survives; it 
just gets shorter. Same for a [favorite](favorites.md): deleting a blueprint drops it out of any saved batch that 
referenced it.
