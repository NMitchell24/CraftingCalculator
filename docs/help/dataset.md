---
title: The Dataset Screen
nav_order: 10
---

# The Dataset Screen

This screen is your library. This is where you will customize the data that you want to use within the Crafting 
Calculator. The Dataset screen allows you to view the three different record types, and a live total of how many you 
have for each. Tap on any card to go to a list of those records where you can view, add, edit and delete them.

## Switching datasets

The droplist at the very top says which dataset you're looking at. Everything below it, and everything
on every other screen, belongs to it. Tap it to switch to another dataset, and use the
![Rename](assets/edit.svg), ![New](assets/add.svg), ![Copy](assets/content-copy.svg) and
![Delete](assets/delete.svg) icons beside it to manage it.

If you only play one game, you can ignore that row entirely and never think about it again.

More detail: [Managing Datasets](managing-datasets.md).

## The record cards

The landing page shows one card per record type:

| Card                                                 | What is in it                                                                              |
|------------------------------------------------------|--------------------------------------------------------------------------------------------|
| ![Categories](assets/label.svg) **Categories**       | Your custom grouping categories. See [Categories](categories.md).                          |
| ![Components](assets/inventory-2.svg) **Components** | Raw materials that are used as crafting components. See [Components](components.md).       |
| ![Blueprints](assets/handyman.svg) **Blueprints**    | Crafting Recipes that you use to make or build something. See [Blueprints](blueprints.md). |

They're in that order for a reason: it's the best order to create them in. A blueprint needs
components to exist first, and both components and blueprints can be linked to a category. Creating these elements 
in this order allows you to make sure that each thing exists before you need to use it. 

Tap the ![Edit](assets/edit.svg) icon to open a list screen for any of the types. You don't have to tap the icon 
directly. Tapping anywhere on the card does the same thing.

> **Note:** There are shortcut icons that match the cards in the [actions bar](actions-bar.md) for easy thumb 
> access on your mobile device. 
> 
> The **![Delete all data](assets/delete-forever.svg) Delete all data** button is 
> covered at the bottom of this page.
>
> The ![Import/Export](assets/import-export.svg) **Import/Export** action opens the screen for backing up and sharing
> your records. See [Import and Export](import-export.md).

## Datasettings

Yes, that's a pun. No, I'm not sorry. (Sure, if we’re being pedantic about the exact linguistic category, it’s a 
portmanteau… but who cares.)

The **Datasettings** card under the record cards holds the settings that belong to the dataset you're in, not to the
whole app. Every game plays by its own rules, so each dataset gets its own copy. Flip a switch in your Rust dataset 
and your Valheim dataset doesn’t budge. Switch datasets and the app follows whatever that dataset is set to.

They also travel with the dataset. [Copying a dataset](managing-datasets.md#copying-one) copies them, and an
[export](import-export.md) carries them along with your records.

The card has two tabs. **General** holds Calculate Yield and Calculate Craft Time. **Economy** holds the money stuff: 
Calculate Costs, Calculate Values and Currency Name.

### Calculate Yield

On by default. This is the switch for everything about recipes that hand you more than one item per craft.

Some games give you exactly one of whatever you craft, every time. Or maybe you just don't care about stacks. Either
way, the yield field is just clutter. Turn **Calculate Yield** off and all of it disappears for this dataset:

- the **Yield per craft** field in the [blueprint editor](blueprints.md#yield-per-craft)
- the **Surplus** tab on the [Craft screen](craft-screen.md)
- the **Surplus Stock** and **Surplus Value** lines in the Crafting Summary
- the **Yield per Craft** and **Surplus** rows on a blueprint's detail card

The math changes too. With Calculate Yield off, every blueprint makes exactly 1 item per craft, whatever its yield says.
That means there's never any surplus, because nothing ever gets overproduced.

**Example:** Minecraft's 1 log → 4 planks, and you need 8 planks.

| Calculate Yield | Crafts | Logs |
|-----------------|--------|------|
| On              | 2      | 2    |
| Off             | 8      | 8    |

Turning it off doesn't erase anything. Every blueprint keeps the yield you gave it, just hidden. Turn it back on and
your planks go right back to 4 per log.

More detail: [How the Math Works](calculations.md).

### Calculate Craft Time

Also on the **General** tab, and also on by default. This is the switch for production time.

Plenty of games never make you wait. Minecraft's crafting table hands you your planks the instant you click, so every
time field in that dataset would just say *Instant* forever. Turn **Calculate Craft Time** off and all of it disappears
for this dataset:

- the **Production time per craft** field in the [blueprint editor](blueprints.md#production-time-per-craft)
- the **Production time per item** field in the [component editor](components.md#production-time-per-item)
- the **Crafting Time** line in the Crafting Summary
- the time at the end of each row on the **Steps** tab of the [Craft screen](craft-screen.md#steps)
- the **Production Time** rows on a detail card, including **Total Production Time** on individual steps

Nothing else in the math changes. Time never feeds into crafts, materials, cost or value, so the rest of the Craft
screen reads exactly the same either way.

**Example:** you smelt 8 iron ore in a Minecraft furnace, at 10 seconds each.

| Calculate Craft Time | Crafting Time | Everything else |
|----------------------|---------------|-----------------|
| On                   | 1:20          | Same            |
| Off                  | Hidden        | Same            |

Like the others, it doesn't erase anything. Every component and blueprint keeps the time you gave it, just hidden.
Turn it back on and your furnace is back to 10 seconds a smelt.

More detail: [How the Math Works](calculations.md#crafting-time).

### Calculate Costs

On the **Economy** tab, and on by default. This is the switch for what your components cost.

Maybe your game has no shop, or you just don't want to price out every rock and stick. Turn **Calculate Costs** off and
all of it disappears for this dataset:

- the **Cost per item** field in the [component editor](components.md#cost-per-item)
- the **Cost** line in the Crafting Summary
- the cost on each row of the **Components** tab on the [Craft screen](craft-screen.md)
- the **Cost per Item** row on a component's detail card

The math changes too. Every component counts as costing 0, whatever you typed in. So a batch costs nothing, and your
Profit is just its Value.

### Calculate Values

Also on the **Economy** tab, and also on by default. It's the mirror image of Calculate Costs: the switch for what your
blueprints are worth.

Turn **Calculate Values** off and this disappears for this dataset:

- the **Value per item** field in the [blueprint editor](blueprints.md#value-per-item)
- the **Value** and **Surplus Value** lines in the Crafting Summary
- the value on each row of the **Surplus** tab on the [Craft screen](craft-screen.md)
- the **Value per Item** row on a blueprint's detail card

Every blueprint counts as worth 0. A batch is worth nothing, so your Profit is whatever it costs, as a negative number.
Calling that "Profit" would be a stretch, so the bottom line of the Crafting Summary is renamed **Cost** instead. It
keeps the minus sign and the red, and it takes the place of the usual Cost line, so you don't see the same number
twice. That's handy if you use cost as an effort score and just want to see how deep in the hole a project puts you.

**Example:** 5 Valheim Bronze, at 2 Copper and 1 Tin each. Copper costs 2, Tin costs 3, and a Bronze is worth 10.

| Calculate Costs | Calculate Values | Cost | Value | Profit                  |
|-----------------|------------------|------|-------|-------------------------|
| On              | On               | 35   | 50    | 15                      |
| Off             | On               | 0    | 50    | 50                      |
| On              | Off              | 35   | 0     | −35 (shown as **Cost**) |
| Off             | Off              | 0    | 0     | 0 (hidden)              |

That's 10 Copper at 2 and 5 Tin at 3 for the cost, and 5 Bronze at 10 for the value.

Turn **both** off and the app stops caring about money at all. The **Profit** line goes too, since it would only ever
say 0, and you're left with a purely component-based calculator: what to gather, how many crafts, and how long it takes.

Like Calculate Yield, neither switch erases anything. Every component keeps its cost and every blueprint keeps its
value, just hidden. Turn them back on and your numbers are right where you left them.

More detail: [How the Math Works](calculations.md#cost-value-and-profit).

### Currency Name

The last thing on the **Economy** tab, and the only one that isn't a switch. Out of the box, the app writes money the
way your device does, so a phone in the US shows $200.00. That's fine for a spreadsheet, but nobody in Rust has ever
paid for anything in dollars.

Type what your game calls its money into **Currency Name**, and every amount in this dataset becomes the number
followed by that name: 200 Gold, 1,500 Scrap, 75 Units. It shows up everywhere the app shows money:

- the **Cost**, **Value**, **Surplus Value** and **Profit** lines in the Crafting Summary
- the cost on each row of the **Components** tab, and the value on each row of the **Surplus** tab, on the
  [Craft screen](craft-screen.md)
- the **Cost per Item** and **Value per Item** rows on a detail card

A few things to know:

- **Twelve characters, tops:** plenty for Gold, Caps, Coins or Mega Credits. Any longer and it starts crowding the
  numbers on a phone.
- **Decimals only when there are some:** hardly any game hands you half a coin, so a whole number doesn't get a .00
  stuck on the end. If an amount does come out to a fraction, you get up to two decimal places, like 12.5 Gold.
- **Emoji work too:** want a coin instead of a word? Type 🪙 and you'll get 200 🪙.
- **Empty means your device's currency:** clear the name and the dataset goes right back to dollars, euros, or
  whatever your phone uses.

**Example:** the same 5 Valheim Bronze, in a dataset where the money is called Coins, like the game's traders use.

| Currency Name        | Cost     | Value    | Profit   |
|----------------------|----------|----------|----------|
| Empty, on a US phone | $35.00   | $50.00   | $15.00   |
| Coins                | 35 Coins | 50 Coins | 15 Coins |

It doesn't touch the math. Only how the numbers are written changes.

## Inside a list

Every record is a card. The top shows its name and its description, if you gave it one. That's it. No clutter, just
the stuff you need to tell your records apart.

Tap ![Info](assets/info.svg) in the corner to see everything else about a blueprint or a component without opening the
editor: what kind of record it is, its category, its numbers, and what goes into it. Categories don't get
![Info](assets/info.svg). A category is just a name and a description, and the card is already showing you both.

Under the line at the bottom of the card are its buttons, and they're the same on a phone, a tablet or a PC.
![Edit](assets/edit.svg) opens the record in the editor. ![Copy](assets/content-copy.svg) opens the editor on a copy
of it; see [Copying a record](dataset.md#copying-a-record). ![Delete](assets/delete.svg) deletes that one record after
a confirmation.

### Search and filter

The ![Search](assets/search.svg) bar at the top filters as you type, matching anywhere in the name. So
`iron` finds `Wrought Iron`, `Iron Ore`, and `Iron Axe`. The **×** clears it.

Next to it, the ![Filter](assets/filter-list.svg) icon narrows by category. See
[Filtering by category](categories.md#filtering-by-category).

### The actions

These [actions](actions-bar.md) show up whenever you go to one of the lists for Categories, Components, or Blueprints.

| Action                                                  | What it does                                |
|---------------------------------------------------------|---------------------------------------------|
| ![New](assets/add.svg) **New**                          | Opens an empty editor for this record type. |
| ![Delete](assets/delete.svg) **Delete**                 | Turns on Delete mode.                       |
| ![Delete all](assets/delete-forever.svg) **Delete all** | Wipes this list after a confirmation.       |

Everything but New grays out when the list is empty, since there would be nothing to act on.

### Copying a record

![Copy](assets/content-copy.svg) **Copy** opens the editor on a **copy** of a record, named `Whatever - Copy`, with
nothing saved yet. Change what you need, fix the name, tap **Save**.

This is the fast way to build out an armor set or a tool tier, where five recipes differ by one
material. The button sits on every card, between Edit and Delete.

### Delete mode (deleting several at once)

Tap ![Delete](assets/delete.svg) **Delete**, then tap every card you want gone. Selected cards will highlight. Then 
tap it a second time to delete all the selected records. You get a confirmation naming how many records are about 
to go.

> **Tip:** Changed your mind? **Press and hold the ![Delete](assets/delete.svg) action** to leave the mode with
> nothing deleted and your selection discarded.

## Editing a record

Tapping ![Edit](assets/edit.svg) on a card opens its editor. The fields differ by type. See [Blueprints](blueprints.md),
[Components](components.md) and [Categories](categories.md) for details about each type. The bar at the bottom is 
always the same:

- ![Delete](assets/delete.svg) **Delete:** the red trash can on the left, only on a record that already exists.
  Removes it, after asking.
- **Cancel:** goes back without saving.
- ![Save](assets/save.svg) **Save:** commits. It stays grayed out until the record has a name, because
  a name is required for each record.

**Your edits are safe from a stray tap.** If you have unsaved changes and you hit Cancel, the back
arrow, or a navigation button, the app stops and asks whether to discard them.

## What deleting actually does

Records reference each other, so it is worth knowing what a delete ripples into:

| You delete…     | …and this happens                                                                         |
|-----------------|-------------------------------------------------------------------------------------------|
| A **category**  | Its components and blueprints survive, and become uncategorized.                          |
| A **component** | It's removed from every blueprint that used it. Those blueprints survive.                 |
| A **blueprint** | It's removed from every blueprint that uses it, and from every favorite that included it. |

Deletes are **permanent** and cannot be undone. This is exactly why every one of them asks you to confirm first. No one 
wants to accidentally delete something they spent the past couple of minutes creating.

## Delete all data

You can delete all data with the ![Delete all data](assets/delete-forever.svg) action on the Dataset landing page. 
This is the sledgehammer that you probably won't use very often, if even at all. But it exists to give you the 
option to clear everything and start over. It removes **every** favorite, blueprint, category and component in the 
dataset you're currently in. Your other [datasets](managing-datasets.md) aren't touched, and the dataset itself 
survives; it's just empty afterwards.

Because deletes are permanent, it doesn't settle for a yes/no box: you have to **type the word
DELETE** to confirm. If a batch was in progress on the Craft screen, it is cleared too, since it would
otherwise be pricing out blueprints that no longer exist.

Use it if your dataset is no longer useful, and you want a clean slate. Don't use it because a list 
looks cluttered. **Delete Mode**, inside the record lists, is the smaller hammer to clean up things you don't need 
anymore. And if you want the dataset gone entirely rather than just emptied, that's
[Delete dataset](managing-datasets.md#deleting-one).
