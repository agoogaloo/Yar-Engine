# Yet Another Raylib (YAR) Engine

Yar Engine is a (very incomplete) game engine for making 2d pixel art games in C#.<br> 
The engine is still very incomplete and not really documented, so I don't recomend using it, but it might still be helpfull as a reference for your own engine/game

---
## Setup
Currently the easiest/only way to use the engine is to add it to an existing C# project as a [git submodule](https://github.blog/open-source/git/working-with-submodules/), or
you can just clone the [example project](https://github.com/agoogaloo/YarEngine-Example) and build off of that.
(its also probably a good idea to use your own fork, so you can easily add all the missing bits to the engine yourself)
<br>
### Adding to a C#/Git Project
1) make a new C# solution and initialize git in whatever folder you're in
   ```
   dotnet new console
   git init
   ```
2) add raylib_cs dependecy
   ```
   dotnet add package Raylib-cs
   ```
4) add Yar Engine as a submodule
   ```
   git submodule add https://github.com/agoogaloo/Yar-Engine
   git submodule update --init --recursive
   ```
6) profit

<br>

alternatively you can just clone the [example project](https://github.com/agoogaloo/YarEngine-Example), run the following command, then build off the template
```
 git submodule update --init --recursive

```

