---
title: Import and Export
nav_order: 9
---

# Import and Export

You spent a whole weekend typing in every ore, ingot and nail Valheim has. It would be a shame if all of that only
ever lived on one phone. This screen is where your records get out of the app, and where somebody else's get in.

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export**

It's two big cards and not much else. **Export Data** gets your records out, and **Import Data** brings them back in.

## Export Data

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export** → ![Export Data](assets/output.svg) **Export Data**

This saves records from the dataset you're in to a file. Keep it as a backup, move it to your PC, or hand it to a
friend who's starting the same game.

The screen opens with **everything selected**. If a full backup is all you're after, tap **Export Data** and you're
done.

### Picking what goes in

Your records are split into four panels: ![Categories](assets/label.svg) **Categories**,
![Components](assets/inventory-2.svg) **Components**, ![Blueprints](assets/handyman.svg) **Blueprints** and
![Favorites](assets/star.svg) **Favorites**. Each header shows how many of its records are selected, and the number
drops as you deselect stuff. Tap a header to open it and see what's inside, sorted by name. A long list scrolls inside
its panel, ten rows at a time.

Tap a row to select or deselect it. Selected rows are highlighted, the same way they are in
[Delete mode](dataset.md#delete-mode-deleting-several-at-once). Tap ![Info](assets/info.svg) on a row to see its
details without changing anything.

The button at the right of each header selects or deselects the whole panel. Its icon tells you where that panel
stands:

- ![All selected](assets/check-circle.svg) **All:** every record in the panel is selected. Tap it to deselect them
  all.
- ![Some selected](assets/indeterminate-check-box.svg) **Some:** some are selected and some aren't. Tap it to select
  the rest.
- ![None selected](assets/check-circle-outline.svg) **None:** nothing in the panel is selected. Tap it to select
  everything.

A panel's header stays highlighted as long as anything in it is selected.

### Why it selects stuff you didn't tap

An export has to make sense on its own. A Bronze Axe with no Bronze in the file is useless to whoever imports it, so
the app keeps everything a record needs selected along with it.

Here's how that plays out with Valheim's bronze chain. Bronze is 2 Copper and 1 Tin, a Bronze Axe is 4 Wood, 8
Bronze and 2 Leather Scraps, and you've saved a favorite called `Bronze Axe run` that holds five axes.

- **Selecting a blueprint:** selects everything it's made from, all the way down. If you select the Bronze Axe and Wood,
  then Bronze, Copper, Tin, Leather Scraps and the categories they're filed under come with it. No questions asked.
- **Selecting a favorite:** does the same for every blueprint in it.
- **Selecting a component:** selects its category too.
- **Deselecting anything else:** deselects everything that needs it, all the way up. If you deselect Copper then  
  Bronze has to go, which means the Bronze Axe goes, which means `Bronze Axe run` goes. That's more than you tapped,
  so the app asks first and tells you how many of each would go.
- **Deselecting a favorite:** only deselects that favorite. Nothing is made out of a favorite.

Categories get one extra trick. A category doesn't need anything, so selecting one only selects the category. If
there are components or blueprints filed under it that aren't selected yet, the app offers to select those too,
along with whatever they need. It's the quick way to export just your `Tools` and nothing else.

> **Tip:** Only want a handful of records? Tap ![All selected](assets/check-circle.svg) on each panel to clear it
> (say yes when it asks), then select the few things you want. Everything they need comes along by itself.

### Exporting

Tap **Export Data**, either the big button under the panels or the ![Export Data](assets/output.svg) action. It
grays out when nothing's selected, and while an export is already running.

The export runs in the background. You'll see "Building your export…" under the button, but you don't have to sit
and watch it. Go check the [Craft screen](craft-screen.md), come back whenever, and this screen will tell you how it
went.

When it's done, a card shows your **latest export**: the dataset it came from, when you made it, the file name, and
where it's saved. That's the newest export on the device, whichever dataset it came from.

> **Note:** If a blueprint you've selected is nested inside itself, the export is refused and the screen names the
> blueprints involved. A file like that could never be imported. Fix the blueprint in the
> [editor](blueprints.md), or deselect it, and try again. You'll only see this on old data, because the editor
> won't let you build a loop anymore.

### Where the file goes

Every export is a `.ccdata` file named after its dataset and the moment you made it, like
`Valheim-20260912-180400.ccdata`.

- **Windows:** the card shows the folder, so you can go and grab it.
- **Android:** it's saved in the app's own storage, where no other app can see it. Use Share to get it out.
- **iPhone and iPad:** open the Files app and go to **On My iPhone → Crafting Calculator → Exports** (it says
  **On My iPad** on an iPad).

The app keeps your **five newest** exports and deletes the oldest one each time you make a sixth. If you delete them
yourself, the card and the Share action disappear until you export again.

### Sharing

Tap ![Share](assets/share.svg) **Share** to send your latest export somewhere: email it to yourself, drop it in your
cloud drive, or send it to a friend. It stays grayed out until there's an export to share.

> **Note:** Exports live in the app's storage, so uninstalling the app or clearing its data deletes them along with
> everything else. An export only counts as a backup once you've shared it somewhere that isn't this device.

## Import Data

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export** → ![Import Data](assets/input.svg) **Import Data**

This goes the other way. Pick a file you exported earlier, or one a friend sent you, and bring its records into the
app. Nothing lands in your data until you've seen what's in the file and said where it goes.

### Picking a file

The screen opens on **Step 1: Select a file**. Tap **Select Import File** in that card and your device's file picker
opens.

- **Windows:** it only shows `.ccdata` files. Your own exports are in the folder the latest export card shows.
- **Android:** it shows every file, because Android has no idea what a `.ccdata` is. Pick the one you want. Anything
  that isn't an export gets turned away in the next step.
- **iPhone and iPad:** your own exports are under **On My iPhone → Crafting Calculator → Exports** (**On My iPad** on
  an iPad). A file somebody sent you is wherever you saved it.

The app makes its own copy of the file and checks it in the background. You'll see "Validating file…" while it works,
and you don't have to sit and wait for it. The file you picked is never changed.

> **Tip:** Built your dataset on your PC and want it on your phone? Export it on the PC, get the file onto the phone
> however you like (email it to yourself, drop it in a cloud drive), then import it there. An export made on one
> platform imports on every other.

### "This file can't be imported"

If something about the file is off, the Step 1 card says so and lists every problem the app found, not just the first.
Nothing was imported. Tap **Choose Another File** to try again, or **Cancel** to clear the list and put the card back
the way it started. Here are the usual suspects.

#### "It says this isn't a Crafting Calculator export file"

You picked something that isn't an export: a screenshot, a world save, a shopping list. It's an easy mistake on Android,
where the picker shows everything. Look for the file ending in `.ccdata`.

#### "It says the file is damaged"

The file got cut off or mangled on its way to you. Usually that's a download that didn't finish, or somebody opened it
in a text editor and saved it back. Ask for it again, or export it again if it's yours.

#### "It says I need to update the app"

The file was made by a newer version of Crafting Calculator than the one you've got. Update the app, then import it
again. Going the other way is never a problem: a file from an older version always imports.

#### "It says something isn't in the file"

A record points at something the file doesn't hold, like a blueprint using a component that isn't there. Crafting
Calculator never writes a file like that, so somebody edited it by hand. Get a fresh export from wherever it came from.

#### "It says a blueprint is nested inside itself"

Somewhere in the file, a blueprint ends up inside its own recipe. There's no way to craft that, so the app won't bring
it in. Fix the blueprint in the dataset the file came from, then export it again.

### Choosing what comes in

Once the file checks out, you're on **Step 2: Choose what you want to import**. That card tells you what you've got:
the dataset the file was exported from, when it was exported, and which version of Crafting Calculator made it.

Under the card, the screen looks just like [Export Data](import-export.md#export-data): the same four panels,
everything selected, and the same rules about what comes along with what. Deselect Copper and it still asks before it
takes the Bronze Axe with it.

When you're happy with it, tap **Import Data**, either the big button under the panels or the
![Import Data](assets/input.svg) action. Wrong file after all? **Choose Another File**, either the button under
**Import Data** or the ![Choose Another File](assets/file-open.svg) action, starts over with a different one. Not
importing anything today? **Cancel**, at the very bottom, drops the file and takes you back to Step 1.

More detail on the panels: [Picking what goes in](import-export.md#picking-what-goes-in).

### As a new dataset, or into this one

The app asks how you want to bring it in:

- **As New Dataset:** makes a brand new dataset out of what you picked. The name comes prefilled with whatever the
  dataset was called in the file, and the usual [unique-name rule](managing-datasets.md#adding-one) applies. You stay in
  the dataset you're in, so switch over when you're ready.
- **Into 'your dataset':** adds what you picked to the dataset you're in right now. The button says its name, so
  there's no guessing where it's going.

As New is the safe bet. Nothing you already have changes, and if you don't like how it turned out you can
[delete it](managing-datasets.md#deleting-one) and pretend it never happened.

### When something's already there

Importing into your current dataset means some of those records might already exist. The app matches them by name,
ignoring capitals and stray spaces, so `copper` in the file is your `Copper`. You'll see "Checking for conflicts…"
while it looks. If nothing clashes, everything goes straight in.

If something does, you land on **Step 3: Resolve conflicts**. The card tells you how many of each kind clash and gives
you three ways to deal with them:

- **Keep Mine:** your records stay exactly as they are, and anything from the file that used its own version uses
  yours instead.
- **Replace Mine:** the file's version overwrites yours: its values, its category and, for a blueprint, its recipe.
  Everything of yours that used the old one keeps using it. It just has the new numbers now.
- **Choose Each:** takes you to **Step 4: Select records to replace**, where you pick them one at a time.

Records that don't clash are added whichever one you pick. Changed your mind? **Cancel** takes you back to picking
records.

### Choosing each one

Step 4 has a panel for each kind of record that clashes, laid out like the ones in
[Picking what goes in](import-export.md#picking-what-goes-in). Open a panel and tap the ones you want replaced. Picked
rows are highlighted, and each header counts your picks. Everything you don't pick stays as it is.

The button at the right of each header picks or unpicks the whole panel, and its icon works the same way it does there:
![All selected](assets/check-circle.svg) when everything's picked, ![Some selected](assets/indeterminate-check-box.svg)
when some of it is, and ![None selected](assets/check-circle-outline.svg) when nothing is. Want the file's version of
every component but only a couple of its blueprints? Tap the Components button, then pick your blueprints one at a
time. Nothing here picks anything you didn't ask for.

When you're done, tap **Import Selected**, either the big button under the panels or the
![Import Selected](assets/input.svg) action. **Cancel** under it takes you back to picking records, and your picks
start over the next time you choose each.

**Example:**

Your Valheim dataset has Copper, Tin, and Bronze made from 2 Copper and 1 Tin. A friend sends you their Bronze Nails,
where one Bronze craft makes 20 nails. Nails need Bronze, so their file has Bronze, Copper and Tin in it too. Their
Copper costs 3 and yours costs 2. You import into your dataset, choose each, and tap only Copper.

| Record       | What you end up with                       |
|--------------|--------------------------------------------|
| Copper       | Your Copper, now costing 3                 |
| Tin          | Yours, untouched                           |
| Bronze       | Yours, untouched: still 2 Copper and 1 Tin |
| Bronze Nails | New, and made from your Bronze             |

> **Note:** Mixing and matching can build a loop neither dataset had. Say your Bronze Nails use a Bronze Plate, and the
> file's Bronze Plate uses Bronze Nails. Keep your Nails, take their Plate, and now each one is inside the other. The
> app catches that before it writes anything, names the blueprints, and leaves you on the list to change a pick.

### When it's done

You get a "Data Imported" toast, or one naming the new dataset, and the screen goes back to **Step 1: Select a file**. If
you imported into the dataset you're in, the [Craft screen](craft-screen.md) picks up anything you replaced without
touching the quantities in your batch.

Every step runs in the background. Leave at any point, come back, and you're right where you left off. To start over on
purpose, keep tapping **Cancel** until you're back on Step 1. Closing the app does it too: a half-finished import
doesn't survive a restart.
