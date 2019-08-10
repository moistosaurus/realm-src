# Realm Source
This is a C# server emulator for the 7.0 vanilla Realm of the Mad God client. Uses NLog for performant logging, and redis for fast in-memory data access.

The source comes setup for local use, compile using an IDE like Visual Studio, and you should be good to go. To play locally, a vanilla RotMG version 7.0 client is required (connecting to localhost). 
- You will find a vanilla 7.0 client, connecting to localhost, in the root folder of the source: [../webmain.swf](https://github.com/moistosaurus/realm-src/webmain.swf)

The aim is for it to eventually become an exact replica of version 7.0 of Realm of the Mad God while offering a great amount of flexibility, allowing for easy modifications in order to make the source your own!

## Why 7.0?
I chose to make the server compatible with version 7.0 because I feel that it is a good balance between the current, and oldschool RotMG, coming with candy features like backpacks and skins, but leaving behind the mess of pets and language strings found in newer clients. And also because the 7.0 client got publicly leaked, fully deobfuscated, making this a lot easier to make possible.

## Ongoing checklist
- [x] NLog for logging
- [ ] Config files
- [ ] Database
- [ ] Request handling for server

If you find a problem in the source, feel free to open an issue [here](https://github.com/moistosaurus/realm-src/issues).

Alternatively, if you'd like to open a pull request, go [here](https://github.com/moistosaurus/realm-src/pulls).

### Deserved credits
Most of the source code is written by me, however snippets have/might be taken here and there (from other private/public sources), as well as inspirations on how the source is structured, which sped up the process by a ton.

For that, credits go to:
- Developers of Revenge of the Fallen
- Developers of Fabiano Swagger of Doom https://github.com/ossimc82/fabiano-swagger-of-doom
- Developers of Nilly's Realm Core https://github.com/cp-nilly/NR-CORE

If you'd like to use the source, I would appreciate if you let the world know that you use this source, it spreads the word around, and helps me poke out potential stability issues that might become occurent in your server.
