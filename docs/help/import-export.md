---
title: Import and Export
nav_order: 9
---

# Import and Export

You spent a whole weekend typing in every ore, ingot and nail Valheim has. It would be a shame if all of that only
ever lived on one phone. This screen is where your records get out of the app, and where somebody else's get in.

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export**

It's two big cards and not much else. **Export Data** works today. **Import Data** is still on its way, so for now
that card just tells you it isn't ready yet. I'm working on it.

## Export Data

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export** → ![Export Data](assets/output.svg) **Export Data**

This saves records from the dataset you're in to a file. Keep it as a backup, move it to your PC, or hand it to a
friend who's starting the same game.

The screen opens with **everything selected**. If a full backup is all you're after, tap **Export Data** and you're
done.

### Picking what goes in

Your records are split into four panels: ![Categories](assets/label.svg) **Categories**,
![Components](assets/inventory-2.svg) **Components**, ![Blueprints](assets/handyman.svg) **Blueprints** and
![Favorites](assets/star.svg) **Favorites**. Each header shows how many records it holds. Tap a header to open it
and see what's inside, sorted by name. A long list scrolls inside its panel, ten rows at a time.

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

![Import Data](assets/input.svg) **Import Data** goes the other way. Pick a file you exported earlier, or one a
friend sent you, and bring its records into the app. It isn't ready yet, so for now the card just says
"Coming Soon!".
