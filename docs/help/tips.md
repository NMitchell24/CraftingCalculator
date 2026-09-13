---
title: Tips and Troubleshooting
nav_order: 14
---

# Tips and Troubleshooting

Everything that didn't fit anywhere else: how to model the awkward bits of real games, and what to
check when a number looks wrong.

## Modeling real games

### Don't model the whole game up front

Model the thing you are actually building this week. A Rust raid kit is maybe fifteen records. A
Valheim base tier is twenty. Add more as you need them, and your dataset grows into exactly the shape
of how you play.

Seriously. Nobody wants to spend several days entering every single resource, crafting recipe and intermediary
component for a game only to have it change the next time the devs decide to *rebalance* it. To be honest, that's
the exact reason why this app ships without any data built in. If the app contained all the data for every popular 
crafting game out there, maintaining that data would be a lot more than a full-time job.

### Stop breaking things down when it stops being interesting

You **don't** have to model every recipe as a [blueprint](blueprints.md). If you always have Bronze, make
Bronze a [component](components.md) with a cost and stop there. The app doesn't know the difference and doesn't care.
It's your data. Create it in the way that's most useful to you.

The point of modeling something as a blueprint is that you want the app to count its components, count its crafting 
steps, track its production time, apply its yield rounding, and report its surplus. If none of that matters for a 
given item, a component is a quarter of the work.

### Watch the yield field

The most common cause of "these numbers are twice what they should be" is a **Yield per craft** left at
1 on a recipe that produces a stack. Planks, arrows, firewood, ammunition, bandages, building pieces —
games love handing you several at once.

See [How the Math Works](calculations.md) for exactly what yield changes.

### Use Cost as an effort score

If your game has no real economy, use cost to represent a level of difficulty instead.
Wood 1, Stone 1, Iron Ore 5, Black Metal 20, boss drop 200.

Now **Cost** means "how much of a grind is this", **Value** means "how much do I want it", and
**Profit** answers the actually interesting question: is this project worth the evening?

### One install, several games

Give each game its own **dataset**. Switching between them swaps out every category, component,
blueprint and favorite in one tap, so your Rust recipes never turn up while you're planning a Valheim
build. See [Managing Datasets](managing-datasets.md).

This also means two games can both have a `Wood` at completely different costs without you having to
invent names to tell them apart.

### Duplicate is your friend for tiers

Armor sets, tool tiers and ammunition types are usually the same basic recipe with one or two materials swapped.
Build the first one properly, then use ![Duplicate](assets/content-copy.svg) **Duplicate** on the
Blueprints list for the rest. See [The Dataset Screen](dataset.md).

### Model a whole build as one blueprint

Nothing says a blueprint has to be a single item. Make one called `2x1 Base Starter` that nests the foundations, 
walls, doors, and everything you need. Then the whole project becomes a single row in your batch with a single
quantity.

Then save it as a [favorite](favorites.md) and it survives the wipe.

## Troubleshooting

### "It wants way more materials than the game does"

Check **Yield per craft** on every blueprint in that branch. This is the answer nine times out of ten.

### "It wants far fewer materials than the game does"

Check that you have not modeled an intermediate item as a **component** when it should be a blueprint.
A component is a dead end. The app stops there and never expands what it's made of.

### "There is surplus I didn't expect"

That's yield rounding, and it's real. You asked for a number that's not a whole multiple of what a
craft produces, so the last craft overshoots. See [Surplus](calculations.md#surplus).

### "Profit ignores my surplus"

On purpose. Surplus is stock, not a sale. The **Surplus Value** line tells you what it's worth if you
do sell it. See [How the Math Works](calculations.md).

### "Crafting Steps are much bigger than my component list"

Also expected. The component list merges every identical component into one row; steps count every visit
to a bench, at every level of the tree. A four-row component list can easily be fifteen crafting steps. If your game 
lets you queue multiple crafts of an item, the steps will tell you how many to queue to get the result you're 
looking for.

### "A blueprint isn't in the list when I add a part"

The blueprint you're editing can't nest something that already nests it, so the **Add component** list
leaves those out. If Ammo uses Gunpowder, then Gunpowder can't use Ammo. See [Blueprints](blueprints.md).

### "A blueprint lost one of its components"

Something it referenced was deleted. Deleting a component removes it from every blueprint that used it,
and deleting a blueprint removes it from every blueprint that nested it and from every favorite that
included it. See [The Dataset Screen](dataset.md).

### "My category filter is not offering a category I made"

The filter menu only lists categories that something in **that list** is actually using. An empty
category has nothing to filter to. Add it to a record and it appears.

### "I can't Save"

![Save](assets/save.svg) **Save** stays grayed out until the record has a name. Name is a required field on every 
record.

### "I changed a component or blueprint and the calculations are wrong"

You had the thing you changed already applied in the blueprint batch before you changed it. The blueprint batches 
are snapshots of those blueprints or components at the point in time when you added them on the Craft screen. If you 
change a blueprint or a component that is currently assigned to an active batch, delete and add it back to see the 
updated information. If it was saved as a favorite, reload the favorite and it will update. These snapshots are 
temporary and won't impact any saved favorites. Saved favorites reference the blueprint directly, not the snapshot.

## Things worth knowing

- **The number in a quantity stepper is editable.** Tap it and type. Much faster than tapping **+** two
  hundred times.
- **Or make the stepper move in tens.** ![Step by 10](assets/x10.svg) **Step by 10** on the
  [Craft screen](craft-screen.md#stepping-by-ten) switches every row over at once, for when you want 40
  arrows and don't feel like typing.
- **A quantity of zero keeps the row.** Handy for asking "what does this batch look like without the
  armor?" without losing your selection.
- **Press and hold ![Delete](assets/delete.svg)** in Delete mode to back out with nothing deleted.
- **Everything is local.** No account, no network, no telemetry. See [Settings](settings.md).
