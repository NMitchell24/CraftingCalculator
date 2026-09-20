---
title: Settings
nav_order: 13
---

# Settings

The ![Settings](assets/settings.svg) icon in the top bar is accessible from anywhere in the app. Tap it again to go
back to whatever you were doing. It remembers where you came from.

It's a short screen. Here's everything you need to know.

## Appearance

Three choices for the app's theme:

- **Follow system:** matches your phone or desktop. Goes dark when your device does, including on a
  schedule if your device is set up that way. This is the default, and it's the right answer for almost
  everyone.
- **Light:** always light, whatever the device says.
- **Dark:** always dark. For playing at 2am without lighting up the room.

The change is instant. There's nothing to save and nothing to restart.

## Exports

**Max history** controls how many export files the app hangs onto. The default is 5, and you can set it anywhere from
1 to 20.

Every time you export, the app counts the files in its Exports folder and deletes the oldest ones until only this
many are left. Since the one you just made is the newest one, it will never be deleted in this process. Set it to 1 and 
each export replaces the last. Set it to 20, and you've got a running history of your last twenty backups.

The number is inclusive, so nothing gets deleted until an export would push you past it.

> **Tip:** A dataset with a few hundred records makes a small file, so 20 of them is nothing next to a single
> screenshot. Turn it up if you like having a trail to fall back to. Turn it down if you only ever want the newest
> one.

Lowering the number doesn't delete anything right away. The extra files stick around until your next export, which
is when the cleanup runs.

More detail: [Import and Export](import-export.md#export-history).

## Diagnostics

The app keeps a log. Not the fun kind you turn into planks. The boring kind that tells me what happened when you
tell me something broke.

There's no crash reporter and no telemetry in this app, so if something goes wrong on your device, I have nothing
to go on unless you hand it to me. That's what this is for. It lives on your phone, it never goes anywhere by
itself, and you can read every line of it.

**View logs** opens it: ![Settings](assets/settings.svg) **Settings** → **View logs**. Newest entries are at the
bottom, and that's where it opens. Tap ![Close](assets/close.svg) **Close** when you're done.

### What's in it

- **The session header:** the app's version and build, your OS version, and your device's manufacturer, model and
  type (phone, tablet, desktop). One of these is written every time the app starts.
- **Errors:** anything that went wrong, what the app was trying to do at the time, and the developer-facing detail
  I need to find it in the code.

### What's never in it

Anything you typed. No component names, no blueprint names, no categories, no dataset names, no numbers you
entered, no file names. Your device's *name* isn't in there either, only its model, so it says `Google Pixel 8`
and not whatever you decided to call the thing.

That's deliberate, and it's enforced twice. The app is written not to put your data in the log in the first
place, and then a second pass scrubs anything that slipped through in a system error message. So you can send me
a log without reading it first, though I'd rather you read it. It's your device.

> **Note:** Your dataset has nothing to do with the log, and the log has nothing to do with your dataset. Deleting
> one doesn't touch the other.

### How big it gets

Capped, and not by much. The app writes to one file until it hits 128 KB, then starts a new one and keeps the two
previous ones. The oldest gets deleted. Three files, about 384 KB total, forever. It can't creep up on your
storage, and it can't fill your device while you're not looking.

The trade-off is that the log only covers recent history. If something broke a month and a hundred launches ago,
that entry is long gone. Grab the log while the problem is fresh.

### Getting it off the device

Where it lives depends on the platform, same as an export:

- **Windows:** the dialog shows the full path. Open that folder and the file is right there.
- **Android:** it's in the app's own storage, where nothing else can reach it, so the dialog gives you a
  ![Download](assets/download.svg) button. Tap it and a copy lands in your **Downloads** folder as a `.txt` file,
  ready to attach to anything.
- **iPhone and iPad:** open the Files app and go to **On My iPhone → Crafting Calculator → Logs** (it says **On My
  iPad** on an iPad). It sits next to the **Exports** folder.

The copy is a snapshot of what you were looking at. The app keeps logging after you take it, so a copy from
yesterday won't have today's errors in it.

## About

The app's name, its version and build number, the copyright, the promise, and the license.

Crafting Calculator is 100% free, and it's staying that way. That's the **privacy and free software commitment**:
no price tag, no ads, no telemetry, no tracking and no data collection. Not in this version, not in the next one,
not ever.

The source code is public under a **personal-use license**. Fork it, build it, tinker with it, make it your own.
You can even post your builds on GitHub. Just don't sell it, and don't put your copy up on an app store or
marketplace. Only Sterling Turd Productions, LLC gets to put the app in the stores. The short version of the terms
is on the screen.

The **fonts** note is there because both typefaces ship inside the app rather than being downloaded:
**Inter** for body text and **Jersey 20** for headings, both under the SIL Open Font License. That's
why the app looks the same offline as online.

## Where your data lives

Everything you create — every component, blueprint, category and favorite — is stored in a database
**on your device**. That means:

- **No account.** There's nothing to sign up for and nothing to log into.
- **No internet.** The app makes no network calls at all. It works in a tent, on a plane, in a basement, or on the moon.
- **No telemetry.** The app doesn't collect anything about you or your dataset, and it doesn't send anything
  anywhere. When I said 100% free, I meant it. Your data is **not** some hidden cost. The app does keep a small
  diagnostic log on your device so I have something to go on when you tell me it broke. It records what the app
  itself did, never your components or blueprints, and it stays on your device unless you decide to send it to
  me. You can read the whole thing.

All of your [datasets](managing-datasets.md) share that one database file, so this applies to every one of
them at once.

It also means that the app comes with an honest caveat: **your dataset lives and dies with the app's storage.** 
I don't keep a cloud copy, so uninstalling the app, or clearing its data from your device's settings, takes your
recipes with it. Your phone's own backup, Google's or Apple's, might have a copy if you've got backups switched on,
but that's between you and your phone. Don't count on it. Worth knowing before you spend an evening entering a
modpack.

The way around that is an export. ![Dataset](assets/menu-book.svg) **Dataset** →
![Import/Export](assets/import-export.svg) **Import/Export** → ![Export Data](assets/output.svg) **Export Data**
saves your records to a file, and ![Share](assets/share.svg) on that file's card gets it off the device and
somewhere safe. See [Import and Export](import-export.md).

If you only want a batch's lists outside the app, the ![Copy](assets/content-copy.svg) **Copy**
buttons on the Craft screen's Components and Surplus tabs put the list on your clipboard, ready to
paste anywhere.

## Getting help

The ![Help](assets/help-outline.svg) icon, next to the ![Settings](assets/settings.svg) in the top bar,
opens the help page for whichever screen you're on. From there, the contents list gets you to every
other page.
