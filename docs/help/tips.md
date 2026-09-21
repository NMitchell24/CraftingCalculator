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

See [How the Math Works](calculations.md) for exactly what yield changes. And if the field isn't there at all,
[Calculate Yield](dataset.md#calculate-yield) is off for this dataset.

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

### Copy is your friend for tiers

Armor sets, tool tiers and ammunition types are usually the same basic recipe with one or two materials swapped.
Build the first one properly, then use ![Copy](assets/content-copy.svg) **Copy** on the
Blueprints list for the rest. See [Copying a record](dataset.md#copying-a-record).

### Model a whole build as one blueprint

Nothing says a blueprint has to be a single item. Make one called `2x1 Base Starter` that nests the foundations, 
walls, doors, and everything you need. Then the whole project becomes a single row in your batch with a single
quantity.

Then save it as a [favorite](favorites.md) and it survives the wipe.

## Troubleshooting

### "It wants way more materials than the game does"

Check **Yield per craft** on every blueprint in that branch. This is the answer nine times out of ten.

Can't find that field? Then [Calculate Yield](dataset.md#calculate-yield) is off, and every recipe counts as making one
per craft. Turn it back on.

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

### "A blueprint isn't in the list when I add a requirement"

The blueprint you're editing can't nest something that already nests it, so the **Add requirements** list
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

### "The app says it couldn't start"

The app couldn't get to your database, so rather than hang on the splash screen forever it says so and stops.
Nothing has been changed and nothing has been deleted; your dataset is still sitting there. Close the app and
open it again, which sorts it out more often than you'd think.

If it keeps doing it, the log is the thing I need. That screen tells you where the log lives, and on Android it
has a **Save log to Downloads** button so you can grab a copy without getting into the app first. More detail:
[Settings](settings.md#diagnostics).

### "A dialog says 'That didn't work'"

Something you tapped didn't happen, so I stopped and said so rather than half doing it. A save, a delete, a
rename, an import: whichever it was, the message names what I couldn't do. Everything you had on screen is still
there, so your edits are still in their fields, your selection is still selected, and your batch is still built.

You'll notice I don't tell you to try again. Tapping it a second time usually gets you the same message, so
there's no point pretending otherwise. If it does keep happening, the log is the thing I need. See
[Settings](settings.md#diagnostics).

### "A screen turned into 'Something went wrong'"

That screen ran into a problem, so the app swapped it for a card that says so instead of sitting there frozen and
pretending everything is fine. It's not you, it's me, and whatever happened is already in the log.

The button on the card is your way out. It says **Back to Craft** everywhere except the [Craft screen](craft-screen.md)
itself, where it says **Try again** and has another go at the same screen. Everything else is untouched: your
dataset, your favorites, and the batch you were building.

If a dialog is what ran into trouble, it closes itself and a red message turns up at the top of the screen to say
so. The screen underneath carries on as it was, with anything you'd typed still in it. Close the message with its
![Close](assets/close.svg) or just move to another screen; it doesn't follow you around.

### "The whole app turned into 'Crafting Calculator ran into a problem'"

Same idea, one level up: something went wrong outside any single screen, so there was no screen left to keep.
Your data is fine, and nothing has been changed or deleted.

- **Try again:** picks up where you were, batch and all. Try this one first.
- **Restart:** reloads the app. It's the bigger hammer, and it costs you the batch you were building.
  Anything you saved is safe.

If either card keeps coming back, the log is the thing I need. See [Settings](settings.md#diagnostics).

## Things worth knowing

- **The number in a quantity stepper is editable.** Tap it and type. Much faster than tapping **+** two
  hundred times.
- **Or step in tens.** Every card on the [Craft screen](craft-screen.md#stepping-by-ten) has
  ![Minus ten](assets/minus-10.svg) and ![Plus ten](assets/plus-10.svg) buttons, for when you want 40 arrows and
  don't feel like typing.
- **A quantity of zero keeps the row.** Handy for asking "what does this batch look like without the
  armor?" without losing your selection.
- **Press and hold ![Delete](assets/delete.svg)** in Delete mode to back out with nothing deleted.
- **Everything is local.** No account, no network, no telemetry. See [Settings](settings.md).
