# Diamond Remover

## Description

A BepInEx plugin for Block Story that removes most functionality relating to diamonds, and tries to add 
more natural replacements for some functions that Diamonds served.

- Removed diamond displays from most, if not all UI elements
- Diamond-related functionality is either inaccessible or removed
- Diamonds cannot drop from enemies or blocks
- Offers involving diamonds are removed from the option pool from stores (I.e, Alchemist's)
- Clicking on timers in the furnace/cauldron will reduce them by 3 seconds per click
- All pet respawn timers are set to 5 minutes, which cannot be manually reduced in-game
- Anvil repair ticks now at minimum repair 1% of an item's max durability to make repairing items with large max durability (such as rings) much more feasible

NOTE: You may still receive diamonds from quests and achievements. This has not been changed.
Using this mod will not alter the diamonds you currently have. Uninstalling/Installing this mod will not remove 
the diamonds you already have.

## Installation 

Download the latest release and move ```DiamondRemover.dll``` into ```/path/to/BlockStory/BepInEx/plugins/```

### Requirements

0. BepInEx properly installed in the BlockStory directory. Installation guide for BepInEx is available <u>[here](https://docs.bepinex.dev/articles/user_guide/installation/index.html).</u>

## Building prerequisites

You'll need the game's assemblies, so you'll need to paste Assembly-CSharp.dll from the game's ```Managed``` folder into ./lib 

## Disclaimer

This software is publicly available in the hope that it will be useful. I do not take responsibility for maintaining or 
improving it in the future.
