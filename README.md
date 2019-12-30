# Realm Source
This is a C# server emulator for the 7.0 vanilla Realm of the Mad God client based on NR Core. Uses NLog for performant logging, and redis for fast in-memory data access.

The source comes setup for local use, compile using an IDE like Visual Studio, and you should be good to go. To play locally, a vanilla RotMG version 7.0 client is required (connecting to localhost). 
- As a little candy, here's a clean, fast and barebones AS3 7.0 client [click me!](https://github.com/moistosaurus/realm-cli) Flex 4.6 recommended.

The aim is for it to eventually become an exact replica of version 7.0 of Realm of the Mad God while offering a great amount of flexibility, allowing for easy modifications in order to make the source your own!
Only difference is that the server will be quite barebones, meaning I will be removing bloated features like packages from the client.
This should allow for it to be more customizable, but also will shorten the development time, and make the client cleaner in general.

If you'd like to run this server on platforms other than Windows, I strongly recommend porting the source to [NET Core](https://dotnet.microsoft.com/download). It is made to be cross-platform, and porting it is a very straightforward process (should only require very minor changes to the actual source code).

Not everything might work correctly. 

## Why 7.0?
I chose to make the server compatible with version 7.0 because I feel that it is a good balance between the current, and oldschool RotMG, coming with good features like backpacks and skins, but leaving behind the mess of pets and language strings found in newer clients. And also because the 7.0 client got publicly leaked, fully deobfuscated, making this a lot easier to make possible.

## Ongoing checklist
- [x] NLog for logging
- [x] Config files
- [x] Resources
- [x] XmlData
- [x] Request handling for server
- [x] Static/export files
- [x] Database
- [x] Get to main menu
- [x] Make 7.0 AS3 client
- [x] Rename .sol and setting paths (so it doesn't try to load prod data)
- [x] Remove GA
- [x] Fix visual bugs with graphics
- [x] Fix font rendering
- [x] Registering and gen. account control (logging in, /char/list, etc.)
- [x] Remove debug console (it's kinda a neat feature, but it's better to just use debugger, so it's just bad bloat)
- [x] Remove age & email verification completely (this is simply not needed here)
- [x] Remove packages
- [x] Remove protips
- [x] Remove surveys
- [x] Remove useless hotkeys
- [x] Remove useless parameters options
- [x] Remove useless client features (e.g. /log/logFteStep & screenshot mode)
- [x] Remove any payment-y stuff (easier & better to make a seperate site for payments)
- [x] Remove useless asset files (e.g. TravisTestingCXML)
- [x] Remove remote textures and data (not needed, simply bloats the client)
- [x] Rework build environments to be more user-friendly (maybe just have a single one...)
- [x] Remove useless data from requests (platforms etc.)
- [x] Remove Steam, Kong. and other platforms that are not needed here
- [x] Remove map loading in the background of main menu (it takes a lot of processing power (20% CPU AFK???))
- [x] Fix /char/list requesting before /account/verify??? (hello?)
- [x] Connect wServer to server (inter server)
- [x] Get in game (includes a lot of stuff, mainly C + P from other sources, will rewrite things later)...
- [x] Optimize client (caching, rendering)
- [x] Make XML configs nicer to work with
- [x] Remove potion purchasing completely
- [x] Remove zombification (including everything todo with it)
- [x] Bring back ammys
- [x] Remove clean text (if you can't take the heat, leave the kitchen)
- [x] Fix item tooltips
- [x] Fix weird merchant purchasing in the 7.0 client (buttons for both gold and fame)
- [x] Fix chat & speech bubbles (???)
- [x] Add all AEs
- [x] Fix legends (/fame/list)
- [x] Update new char slot cost properly
- [x] Fix containers
- [x] Don't play sound FX and music in the background if they are disabled (helps performance)
- [x] Fix up good vault/nexus maps
- [x] Add DungeonGen
- [x] Remove LootDrop, LootTier & XpBooster boosters
- [x] Add handlers for locking/ignoring
- [x] Refine /app/globalNews
- [x] Remove fame notifications
- [x] Fix condition effects
- [x] Remove packet priority (everything should be sent logically, no priorities needed)
- [x] Make particles render in a static way (only try to update texture on update, not every frame, performance boost)
- [x] Make it possible to control char slot currency through server (XML tag)
- [x] Make dialogs/result packets nicer to work with (e.g. merchant dialogs) - added FlexibleDialog.as
- [x] Fix up connecting bugs and remove NR con queue
- [x] Fix map editor testing & visual bugs
- [x] Fix cloak (& possibly others) tooltip not working when they are equipped
- [x] Fix client loading up wrong character (causes skin view to bug out)
- [x] Fix skin ownership parsing
- [x] Fix dye merchants not showing the look of the dye 
- [x] Remove gifts completely (really want this source to be barebones!)
- [x] Fix small network latency even on localhost (Tasks are bad, try reset events.)
- [x] Add tab switching like on prod (really important!!!)
- [x] Fix merchant taking gold if inv is full
- [x] Fix vault chest purchashing
- [x] Fix interact key always entering ghall
- [x] Make PacketIds ordered
- [x] Fix "skin not owned" after buying a skin in game
- [x] Setup NLog properly for dynamic log dir
- [x] Add random realm naming
- [x] Fix UsePortal disposing map when entering a portal that isn't implemented
- [x] Fix Loot bags
- [x] Fix con retry issue with client socket data
- [x] Remove client sided /help command
- [x] Fix Ally Shoot issues
- [x] Fix Scrollbars (make them scrollable via mouse wheel)
- [x] Allow to view skins when no char slots available like on prod
- [x] Add Hardware Accel (old clients don't have it, this will be quite difficult...)
- [x] Add unnecessary particles option
- [x] Don't use queued status texts (why is this even a thing...)
- [x] Add hp bars option (make them prettier tho...)
- [x] Add option to disable Ally Shots (server player shoot as well unless its main player ID)
- [x] Add option to disable Ally Damage (damage dealt to and by allies on enemies/themselves)
- [x] Add option to disable Ally notifications (EXP & noti packet)
- [ ] Add server sided projectile, AOE, and ground damage tracking (fix godmode basically)
- [x] Fix projectile Z value when standing on ProtectFromGroundDamage objects
- [x] Add guild halls
- [x] Add permapets (yay)
- [x] Add realm behaviors
- [ ] Add Abyss of Demons
- [ ] Add Undead Lair
- [ ] Add Snake Pit
- [ ] Add Spider Den
- [ ] Add Ocean Trench
- [ ] Add Mad Lab
- [ ] Add Sprite World
- [ ] Add Pirate Cave
- [ ] Add Forbidden Jungle
- [ ] Add Beachzone
- [ ] Add Candyland
- [ ] Add Tomb of the Ancients
- [ ] Add Davy's Jones Locker
- [ ] Add Manor of the Immortals
- [ ] Add Haunted Cemetery
- [ ] Add Oryx's Castle
- [ ] Add Oryx's Chamber
- [ ] Add Wine Cellar
- [ ] Add Cave of a Thousand Treasures

If you find a problem in the source, feel free to open an issue [here](https://github.com/moistosaurus/realm-src/issues).

Alternatively, if you'd like to open a pull request, go [here](https://github.com/moistosaurus/realm-src/pulls).

## Deserved credits
Server code is mostly just NR Core with some changes here and there and cleanup to work with the provided client, can't take much credit for that. The client is completely managed by me though.

For that, credits go to:
- Developers of Nilly's Realm Core https://github.com/cp-nilly/NR-CORE
- Skilly

If you'd like to use the source, I would appreciate if you let the world know that you use this source, it spreads the word around, and helps poke out potential stability issues that might become occurent in your server.
