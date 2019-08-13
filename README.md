# Realm Source
This is a C# server emulator for the 7.0 vanilla Realm of the Mad God client. Uses NLog for performant logging, and redis for fast in-memory data access.

The source comes setup for local use, compile using an IDE like Visual Studio, and you should be good to go. To play locally, a vanilla RotMG version 7.0 client is required (connecting to localhost). 
- As a little candy, here's a clean, fast and barebones AS3 7.0 client [click me!](https://github.com/moistosaurus/realm-cli). Flex 4.6 recommended.

The aim is for it to eventually become an exact replica of version 7.0 of Realm of the Mad God while offering a great amount of flexibility, allowing for easy modifications in order to make the source your own!
Only difference is that the server will be quite barebones, meaning I will be removing bloated features like packages from the client.
This should allow for it to be more customizable, but also will shorten the development time, and make the client cleaner in general.

If you'd like to run this server on platforms other than Windows, I strongly recommend porting the source to [NET Core](https://dotnet.microsoft.com/download). It is made to be cross-platform, and porting it is a very straightforward process (should only require very minor changes to the source code).

## Why 7.0?
I chose to make the server compatible with version 7.0 because I feel that it is a good balance between the current, and oldschool RotMG, coming with candy features like backpacks and skins, but leaving behind the mess of pets and language strings found in newer clients. And also because the 7.0 client got publicly leaked, fully deobfuscated, making this a lot easier to make possible.

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
- [ ] Remove map loading in the background of main menu (it takes a lot of processing power)
- [ ] Get in game (long process...)
- [ ] Optimize client (caching, rendering)

If you find a problem in the source, feel free to open an issue [here](https://github.com/moistosaurus/realm-src/issues).

Alternatively, if you'd like to open a pull request, go [here](https://github.com/moistosaurus/realm-src/pulls).

## Deserved credits
Many parts of the source code have been written by me, however code has been taken here and there (from other private/public sources), as well as inspirations on how the source is structured, which sped up the process by a ton.

For that, credits go to:
- Developers of Fabiano Swagger of Doom https://github.com/ossimc82/fabiano-swagger-of-doom
- Developers of Nilly's Realm Core https://github.com/cp-nilly/NR-CORE
- Developers of Revenge of the Fallen

If you'd like to use the source, I would appreciate if you let the world know that you use this source, it spreads the word around, and helps poke out potential stability issues that might become occurent in your server.
