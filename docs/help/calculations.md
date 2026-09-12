---
title: How the Math Works
nav_order: 11
---

# How the Math Works

Every number on the [Craft screen](craft-screen.md) comes from a handful of small, boring rules. None
of them are clever. All of them are worth knowing. Once you understand them you can tell at a glance
whether a surprising number is a bug in your dataset or the game genuinely being like that.

Here they all are, with examples of how they work.

## Crafts, not items

The rule everything else hangs from:

> **A craft is indivisible.** You cannot craft half of a Stone Axe.

So when the app needs items, it works out how many **crafts** it will take, and it always rounds *up*:

```
crafts = round up ( quantity needed / yield per craft )
```

If your blueprint yields 1, crafts and items are the same number. If it yields more, they come apart.

**Example:** Minecraft planks: 1 log yields **4** planks.

| You need | Crafts | You get | Left over |
|----------|--------|---------|-----------|
| 4 planks | 1      | 4       | 0         |
| 5 planks | 2      | 8       | 3         |
| 6 planks | 2      | 8       | 2         |
| 8 planks | 2      | 8       | 0         |
| 9 planks | 3      | 12      | 3         |

Ask for any quantity of planks that's not a multiple of four, and you pay for a whole extra log. That's not the app
being fussy, that's Minecraft. The app just lets you see it coming.

## Surplus

The leftovers from that rounding.

```
surplus = ( crafts × yield ) − quantity needed
```

Every level of the tree contributes. Ask for 6 planks and get 2 spare. If those planks feed a recipe
that itself yields 3, you get spares of *that* too, and both show up.

The **Surplus** tab lists all of it, merged across the whole batch, with the value for each.

**Surplus value is reported separately and never part of Profit.** Deliberately: surplus is extra crap you didn't 
really ask for. It's just the result of how that particular game works. You can stash it for later, discard it, or 
sell it. That's up to you. The app won't assume that you plan on selling it.

## How quantities travel down the tree

A child's quantity is multiplied by its parent's **crafts**, not by the parent's item count.

```
child quantity = child quantity per craft × parent crafts
```

This is what makes yield pay off all the way down. If a parent yields 2, you run half as many crafts,
so all the requirements beneath are cut in half too.

**Example:** A Valheim tree, asking for **3 Bronze Axes**:

```
Bronze Axe         x3   -> 3 crafts (yield 1)
  Bronze           x24  -> 24 crafts (8 per axe x 3 crafts)
    Copper         x48  (2 per bronze x 24 crafts)
    Tin            x24  (1 per bronze x 24 crafts)
  Wood             x12  (4 per axe x 3 crafts)
  Leather Scraps   x6   (2 per axe x 3 crafts)
```

The **Components** tab shows the bottom line only — 48 Copper, 24 Tin, 12 Wood, 6 Leather Scraps — merged and
sorted. The **Steps** tab shows the whole shape above, with ![Blueprint](assets/handyman.svg) marking
each blueprint you craft and ![Component](assets/inventory-2.svg) each component you gather.

If two different blueprints in your batch both need Wood, the Components tab gives you **one** Wood row
with the total. That merge is the entire reason this app was created. It answers the question: *"How much crap do I 
have to get if I want to make all of these things?"* with one total per component.

> **Note:** The smelting steps for Copper and Tin were deliberately left out for the sake of simplifying the example.

## Crafting Steps

**Every craft. At every depth. Counted once.**

```
crafting steps = sum of the craft count of every blueprint in the tree
```

Components don't count into steps. You gather, harvest, or purchase them. 

From the example above: 3 Bronze Axe crafts + 24 Bronze crafts = **27 crafting steps**. That's
genuinely 27 items that need to be crafted. 

Yield reduces this directly. If Bronze yielded 2 per craft, you would need 12 Bronze crafts instead of
24, and the whole project would drop to 15 steps.

## Cost, Value and Profit

Three separate ideas, and the difference matters.

```
Cost   = for every raw component: cost per item × total quantity
Value  = for every blueprint in your batch: value per item × batch quantity
Profit = Value − Cost
```

Three things to note:

1. **Cost only counts components.** Blueprints have a Value, not a Cost. Their cost *is* whatever
   their components add up to. Nothing gets counted twice.
2. **Value only counts the blueprints you actually asked for.** A nested blueprint's own Value isn't
   added, because you're not selling the Bronze. The Bronze was consumed during crafting. There's 
   nothing else to sell but the Axe you made out of it.
3. **Surplus value is completely separate.** See [Surplus](calculations.md#surplus) above.

If you left every cost and value at zero, all three lines read zero. That's a perfectly reasonable way
to use the app.

## Crafting Time

```
Crafting Time  =  for every blueprint in the tree:  time per craft × its crafts
               +  for every raw component:  time per item × total quantity
```

In other words: how long the whole project takes if you did every single step yourself, one after
another. It doesn't account for parallel smelters, and it doesn't factor in your friend helping.

Note the asymmetry, which isn't an oversight:

- **Blueprint time is per craft**, so it scales with **crafts**. A recipe that yields 4 in ten seconds
  costs ten seconds, not forty.
- **Component time is per item**, so it scales with **quantity**. Components have no yield. You gather
  them. 

Times are shown in the smallest form that fits:

| Reads as          | Means                                    |
|-------------------|------------------------------------------|
| `Instant`         | What it says                             |
| `5.5s`            | Under a minute, to the tenth of a second |
| `4:30`            | Minutes and seconds                      |
| `2:04:30`         | Hours, minutes, seconds                  |
| `3 Days 02:04:30` | A day or more                            |

> **Note:** Once the time required goes over 1 minute the display will no longer show fractional seconds. It will 
> always round down to the nearest second. The fractions still count toward the math, they just aren't shown in the 
> Crafting Summary or Steps breakdown.
> 
> Example: You have a blueprint that takes 30.5s.  
> A Quantity of 2 will show 1:01 (Exact)  
> A Quantity of 3 will show 1:31 (Rounded down)

## Components Needed

Every raw component in the batch, added together into one number. It counts *items*, not distinct
kinds. 48 Copper, 24 Tin, 12 Wood and 6 Leather Scraps is 90 total Components Needed, from four rows.

It is a rough gauge of "how much inventory space and how many trips", not a precise one, since your
game's stack sizes are its own business.

## A few edge cases, explained

**A quantity of zero.** Perfectly legal. The blueprint stays in your batch and contributes nothing to
any total. This is useful for toggling a line item on and off without losing your place. Stepping *below* zero
removes the row entirely.

**Yield below 1.** Not possible. The editor floors it at 1, because a recipe producing nothing per
craft has no meaning and it borks the math.

**Negative production time.** Not possible. This would cause it to subtract from the batch total time. You can't 
craft something that gives you time back in your day. So use your time wisely.

**A recipe that loops back on itself.** Not possible. If blueprint A nests B and B nests A, there's no
bottom to the tree, so there's no total to add up. The editor won't offer you a blueprint that already
uses the one you're editing, so the loop never gets made. I'm pretty sure no game needs one anyway, for
exactly the same reason. More detail: [Blueprints](blueprints.md).
