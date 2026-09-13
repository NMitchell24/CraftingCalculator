---
title: Managing Datasets
nav_order: 8
---

# Managing Datasets

A **dataset** is everything you've taught this app about one game. Its categories, its components, its
blueprints, and the favorites you saved. You can have as many as you like, and switch between them in a
tap.

This matters because "Wood" isn't the same thing in Valheim as it is in Rust. Keep a dataset per game and 
each one stays exactly as big as that game needs it to be.

> **Note:** If you've been using this app for a while, everything you already built is sitting in a
> dataset called **Default**. Nothing moved, and nothing was lost. Rename it to whatever game it's
> actually for and carry on.

## Switching between them

The droplist at the top of the ![Dataset](assets/menu-book.svg) **Dataset** screen is the whole
feature. Tap it, pick a dataset, and the entire app reloads onto that one: the record lists, the
[Craft screen](craft-screen.md), your [favorites](favorites.md), the category filters, all of it.

The app remembers which dataset you picked. Close it, come back tomorrow, and you're still in the same
one you were working in.

> **Note:** Switching clears whatever batch you had going on the [Craft screen](craft-screen.md). It
> has to. That batch was built out of blueprints from the dataset you just left, and they don't exist
> in the one you just arrived in. Save it as a [favorite](favorites.md) first if you want it back.

## What a dataset carries

| Lives in the dataset                                 | Shared across all of them        |
|------------------------------------------------------|----------------------------------|
| ![Categories](assets/label.svg) Categories           | Your theme choice                |
| ![Components](assets/inventory-2.svg) Components     | The help you're reading now      |
| ![Blueprints](assets/handyman.svg) Blueprints        |                                  |
| ![Favorites](assets/star.svg) Favorites              |                                  |

Two datasets can happily use the same name for different things. A `Wood` component in your Rust dataset and a
`Wood` component in your Valheim dataset are separate records with separate costs, and neither one knows the
other exists.

## Adding one

![Dataset](assets/menu-book.svg) **Dataset** → ![New](assets/add.svg) **New dataset**

Type a name, tap **OK**, and you're dropped straight into it. It starts completely empty, which means
the [Dataset screen](dataset.md) will show zeroes across the board and the Craft screen will have
nothing to offer you. That's expected. Head to [Getting Started](getting-started.md) and build it up the
same way you built the first one.

Names have to be unique. If you try to reuse one, the app says so and hands your text back to you to
edit rather than making you type the whole thing again.

> **Tip:** Starting a modded run, or a new wipe with different rules? Make a second dataset instead of
> editing the one you already trust. Your old numbers stay intact, and you can flip between them to
> compare. And if the new one is mostly the same as the old one, copy it rather than starting empty.

## Copying one

![Dataset](assets/menu-book.svg) **Dataset** → ![Copy](assets/content-copy.svg) **Copy dataset**

Builds a new dataset that starts out as a duplicate of the one you're in. Every category, every
component, every blueprint, and every favorite, all of it copied across.

This is what you want when the second dataset is *mostly* the same as the first. Say you've spent a week
building out `Rust - Vanilla`, and then you join a modded server that has custom Crafting Recipes. 
Copy it, call the copy `Rust - Modded`, and change the things that actually differ. That beats 
retyping two hundred components.

The name comes prefilled as the original with `- Copy` on the end, the same way
[duplicating a record](dataset.md#duplicate-mode) does, which also parks the copy right next to the
original in the droplist. Type over it with whatever you want. The unique-name rule still applies.

From there the two are completely separate. Changing the `Beancan Grenade` recipe in the copy does nothing to 
`Beancan Grenade` in the original, changing a cost in one doesn't change it in the other, and deleting either 
one leaves the other standing. They just happen to have started out identical.

You're dropped straight into the copy when it's done, because that's the one you probably want to edit.

> **Note:** A big dataset takes a moment to copy, and you'll get a spinner while it works. It's writing a
> fresh row for every single record you have. Let it finish.

## Renaming one

![Dataset](assets/menu-book.svg) **Dataset** → ![Rename](assets/edit.svg) **Rename dataset**

Renames whichever dataset is currently selected. Nothing inside it changes; it's just a label. The same
unique-name rule applies.

## Deleting one

![Dataset](assets/menu-book.svg) **Dataset** → ![Delete](assets/delete.svg) **Delete dataset**

This deletes the selected dataset **and everything in it**: every category, component, blueprint and
favorite it holds. Your other datasets aren't touched.

Because that's a lot of work to throw away, it doesn't settle for a yes/no box. You have to **type the
word DELETE** to confirm, the same as
[Delete all data](dataset.md#delete-all-data) does.

You can't delete your last dataset. One is always selected, so there has to be one to select. If you
want a clean slate without losing the dataset itself, use
![Delete all data](assets/delete-forever.svg) **Delete all data** instead. That empties the current
dataset and leaves it standing.

## Backing one up or sharing it

![Dataset](assets/menu-book.svg) **Dataset** → ![Import/Export](assets/import-export.svg) **Import/Export**

This is where exporting a dataset to a file, and importing one back in, is going to live. It isn't finished yet, so
for now both of its cards just say "Coming Soon!". See [Import and Export](import-export.md).

## Where they're stored

All of your datasets live side by side in the same database file on your device. There's still no
account, no cloud, and no sync. That also means the caveat from [Settings](settings.md) applies to all
of them at once: uninstall the app or clear its data, and every dataset goes with it.

More detail: [The Dataset Screen](dataset.md).
