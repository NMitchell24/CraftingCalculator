---
title: Welcome
nav_order: 1
---
# Welcome to Crafting Calculator

You know the moment. You've decided to build the thing. The big thing. A beacon in **Minecraft**, enough rockets
and C4 to foundation wipe that massive clan compound in **Rust**, as many stasis devices as you can hold in **No 
Man's Sky**. You open the crafting bench, and the game cheerfully informs you that you need 4 of something that 
needs 12 of something else. Those things each require 2 of this one thing and 8 of that other thing, and it all 
boils down to a truly upsetting amount of raw materials.

So you do what everyone does: you tab out, you find a wiki, you open the calculator app, you lose count, you create a 
spreadsheet, but somehow you still manage to mine the wrong thing for forty minutes.

That's where this app comes in. Ditch the spreadsheet and put the crafting recipes in here instead. You teach it your 
game once, and from then on you point at what you want to build and how many of those things you need. This app then 
tells you exactly how many raw resources you need, and most importantly, how much you stand to profit.

## Three records to rule them all

Everything here is built on three generic records that are common to Survival Crafting games.

| Word          | What it means                                                                                                                                               | In your game                              |
| ------------- |-------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------|
| **Component** | Raw stuff. Something you gather, mine, kill, farm, buy, or something you can craft but you'd rather track as a component instead of a blueprint.            | Wood, Stone, Iron Ore, Fiber, Leather     |
| **Blueprint** | A recipe. A list of components, or other blueprints, that makes the thing you need. Or even one of the things you need to make the thing you *really* need. | Iron Nails, Fine Wood Bow, Low Grade Fuel |
| **Category**  | A label you stick on components and blueprints so you can find them later.                                                                                  | Ores, Food, Base Building, Tier 3 Gear    |

All three live inside a **dataset**, which is just everything you've taught the app about one game. Start
with one and forget it exists. Play a second game, and you can add a second dataset, so a Rust Rocket never
turns up while you're costing out a Valheim longship. See [Managing Datasets](managing-datasets.md).

Probably the *most* important part is that little comma clause in the Blueprint row: **a blueprint can contain other 
blueprints**. In Rust, a Rocket needs 10 Explosives, 150 Gunpowder, and 2 Metal Pipes. Explosives and Gunpowder are 
also things you can craft. They are their own **Blueprints**. You describe each recipe once, exactly the way the 
game defines it, and the app does all the math on your behalf, all the way down to the raw ore. The **Calculator** 
part is what allows you to then say "I need 100 of these, how much do I need to grind?" and this app gives you the 
answer.

## Where to go next

* Brand new and staring with an empty app? Head over to [Getting Started](getting-started.md).

* Want to know what every button on the main screen does? [The Craft Screen](craft-screen.md).

* Curious how it works out surplus, steps and profit? [How the Math Works](calculations.md).

* Everything is stored on your device. No account, no sign-up, no internet, no telemetry. You have full control over 
  how your data is used and who can see it.

Help is always one tap away. The ![Help](assets/help-outline.svg) icon in the top bar opens the help page for whatever screen you are 
looking at.
