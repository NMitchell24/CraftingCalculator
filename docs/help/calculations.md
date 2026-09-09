---
title: How the Math Works
nav_order: 9
---

# How the Math Works

Every number on the [Craft screen](craft-screen.md) comes from a handful of small, boring rules. None
of them are clever. All of them are worth knowing, because once you know them you can tell at a glance
whether a surprising number is a bug in your dataset or the game genuinely being like that.

Here they all are, with worked examples.

## Crafts, not items

The rule everything else hangs off:

> **A craft is indivisible.** You cannot pull a lever two-thirds of the way.

So when the app needs items, it works out **crafts**, and it always rounds *up*:

```
crafts = round up ( quantity needed / yield per craft )
```

If your blueprint yields 1 — which most do — crafts and items are the same number and you can stop
reading this section. If it yields more, they come apart.

**Worked example.** Minecraft planks: 1 log yields **4** planks.

| You need | Crafts | You get | Left over |
|---|---|---|---|
| 4 planks | 1 | 4 | 0 |
| 5 planks | 2 | 8 | 3 |
| 6 planks | 2 | 8 | 2 |
| 8 planks | 2 | 8 | 0 |
| 9 planks | 3 | 12 | 3 |

Ask for one plank more than a multiple of four and you pay for a whole extra log. That is not the app
being fussy — that is Minecraft, and now you can see it coming.

## Surplus

The leftovers from that rounding.

```
surplus = ( crafts × yield ) − quantity needed
```

Every level of the tree contributes. Ask for 6 planks and get 2 spare; if those planks feed a recipe
that itself yields 3, you get spares of *that* too, and both show up.

The **Surplus** tab lists all of it, merged across the whole batch, with what it is worth.

**Surplus value is reported separately and is never part of Profit.** Deliberately: surplus is stock
sitting in a chest, not money in your pocket. If you sold it, it would be value; until then it is a
reminder to check your storage before starting the next project. Fold it into Profit and every batch
would look better than it is.

## How quantities travel down the tree

A child's quantity is multiplied by its parent's **crafts** — not by the parent's item count.

```
child quantity = child quantity per craft × parent crafts
```

This is what makes yield pay off all the way down. If a parent yields 2, you run half as many crafts,
so everything beneath it halves too.

**Worked example.** A Valheim-flavoured tree, asking for **3 Bronze Axes**:

```
Bronze Axe  x3   -> 3 crafts (yield 1)
  Bronze    x12  -> 12 crafts (4 per axe x 3 crafts)
    Copper Ore x24  (2 per bronze x 12 crafts)
    Tin Ore    x12  (1 per bronze x 12 crafts)
  Wood      x6   (2 per axe x 3 crafts)
```

The **Components** tab shows the bottom line only — 24 Copper Ore, 12 Tin Ore, 6 Wood — merged and
sorted. The **Steps** tab shows the whole shape above, with ![Blueprint](assets/handyman.svg) marking
each blueprint you craft and ![Component](assets/inventory-2.svg) each component you gather.

If two different blueprints in your batch both need Wood, the Components tab gives you **one** Wood row
with the total. That merge is the entire point of the shopping list.

## Crafting Steps

**Every craft, at every depth, counted once.**

```
crafting steps = sum of the craft count of every blueprint in the tree
```

Components are not steps. You gather those; you do not craft them.

From the example above: 3 Bronze Axe crafts + 12 Bronze crafts = **15 crafting steps**. That is
genuinely 15 trips to a bench, which is a much more honest measure of "how long is this going to take"
than the four rows the shopping list has.

Yield reduces this directly. If Bronze yielded 2 per craft, you would need 6 Bronze crafts instead of
12, and the whole project would drop to 9 steps.

## Cost, Value and Profit

Three separate ideas, and the difference matters.

```
Cost   = for every raw component:  cost per item × total quantity
Value  = for every blueprint in your batch:  value per item × batch quantity
Profit = Value − Cost
```

Three things to note:

1. **Cost only counts components.** Blueprints have a Value, not a Cost — their cost *is* whatever
   their components add up to. Nothing gets counted twice.
2. **Value only counts the blueprints you actually asked for.** A nested blueprint's own Value is not
   added, because you are not selling the Bronze — you are selling the Axe you made out of it.
3. **Surplus value is not in there.** See [Surplus](#surplus) above.

If you left every cost and value at zero, all three lines read zero. That is a perfectly reasonable way
to use the app; the shopping list works just as well.

## Crafting Time

```
Crafting Time  =  for every blueprint in the tree:  time per craft × its crafts
               +  for every raw component:  time per item × total quantity
```

In other words: how long the whole project takes if you did every single step yourself, one after
another. It does not model parallel smelters, and it does not model your friend helping.

Note the asymmetry, which is not an oversight:

- **Blueprint time is per craft**, so it scales with **crafts**. A recipe that yields 4 in ten seconds
  costs ten seconds, not forty.
- **Component time is per item**, so it scales with **quantity**. Components have no yield — you gather
  them one at a time.

Times are shown in the smallest form that fits:

| Reads as | Means |
|---|---|
| `Instant` | Zero |
| `5.5s` | Under a minute, with tenths |
| `4:30` | Minutes and seconds |
| `2:04:30` | Hours, minutes, seconds |
| `3 Days 02:04:30` | A day or more |

## Components Needed

Every raw component in the batch, added together into one number. It counts *items*, not distinct
kinds — 24 Copper Ore and 12 Tin Ore is 36 Components Needed, from two rows.

It is a rough gauge of "how much inventory space and how many trips", not a precise one, since your
game's stack sizes are its own business.

## A few edge cases, explained

**A quantity of zero.** Perfectly legal. The blueprint stays in your batch and contributes nothing to
any total — useful for toggling a line item on and off without losing your place. Stepping *below* zero
removes the row entirely.

**Yield below 1.** Not possible. The editor floors it at 1, because a recipe producing nothing per
craft has no meaning and would make the arithmetic undefined.

**Negative production time.** Also not possible, and for the same reason — it would subtract from the
batch total.

**A recipe that loops back on itself.** If blueprint A nests B and B nests A, there is no bottom to the
tree. Rather than spinning forever, the app stops at a depth of 64 and tells you which blueprint the
loop involves. If you ever see that message, look for a recipe you accidentally pointed at one of its
own ancestors.
