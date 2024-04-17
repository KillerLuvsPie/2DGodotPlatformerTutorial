# ABOUT

This project is some sort of self-imposed tutorial with the purpose of learning the basics of the Godot API, programming workflow and the editor's capabilities, such as nodes, signals and scenes. The assets present in this project are completely free to use, check the credits section on how to find them.

This is a 2D platformer with 5 levels, all with distinct level designs, made with the Godot engine.

# GAMEPLAY FLOW
- Only level 1 is available to play in the beginning
- The game gets harder each level
- The last level is a boss battle
- The player traverses the level and wins by collecting all of the fruit in it while avoiding all the enemies and traps
- The game registers the seconds it took to complete the level, as well as how many times the player died

# "CREATED BY ME" ASSETS
- Scripts
- Scenes (Includes levels and prefabs)
- Custom resources (Used to save key bindings)
- Minimal use of themes (alter the appearance of certain Godot game components such as dropdown boxes)

# MECHANICS
- Use of a singleton autoload node (named Global) to store general game variables, asset references and functions
- Game Manager to control general game behavior
  - UI control
    - Timer
    - Fruit Counter
    - Pause menu
    - Win menu
    - Lose menu
    - Fade in/out effect
  - Game state control such as pausing and game start countdown
  - Music playback control
  - Saving statistics to the Global class
- Basic player movement
  - Acceleration based movement
  - Hold to jump higher
- Animations and sound effects playing during specific actions and events for both player and enemy characters
- Pickups (fruit)
- Enemies
  - Pigs (walk horizontally and switch direction upon collision with terrain or reaching a set position)
  - Birds (fly inside a direction vector and smoothly flip directions upon reaching either point)
  - Tree trunks (stand still and shoot projectiles every X seconds)
  - Rock Head (attempts to smash the player character when it enters an area)
  - Rhino (Part of the boss battle, charges at either the player or the boss when the gates are open and either are in range)
  - Bird boss (Will fly down and slam the ground every X seconds. The lower the health, the faster and more aggressive it becomes. Can open the rhino gates if it slams the switches)
- Traps
  - Falling (only on level 3)
  - Spinning spike ball (counter rotation animation technique. Spins the object from a pivot point)
  - Circular saw (draws the path at runtime. Moves from point A to B and switches direction if looping is off)
- Moving platforms
  - Carrousel (spin platforms around a pivot point)
  - Path (move a platform along a path that is drawn at runtime)
- Save system using Binary writer/reader
- Main menu
- Options menu
  - Window mode option (change how window is displayed)
  - Resolution (change window resolution. Due to how Godot handles resolutions, only works with windowed mode)
  - Volume sliders for music and sound effects (Changes corresponding audio bus volume)
  - Key remapping system (programmed to allow customization of more than one set of controls. Only one set for this project. Saves controls with a custom resource, same location as save file)

# ADDITIONAL NOTES
- Use of signals (both standard Godot signals and custom signals)
- Tilemaps (used to quickly create tile-based levels)
- Collision layers and masks (choose what gets detected by which object)

# CREDITS
Scripting and editor work: Killer_Luvs_Pie (me)

Artwork: Pixel Frog - Pixel adventure and Pixel Adventure 2 free sprites package (downloaded from [itch.io](https://pixelfrog-assets.itch.io/pixel-adventure-1)

Music: Juhani Junkala - Chiptune Adventure package
(downloaded from [opengameart.org](https://opengameart.org/content/4-chiptunes-adventure)

Sound Effects: Juhani Junkala - The Essential Retro Video Game Sounds Collection
(downloaded from [opengameart.org](https://opengameart.org/content/512-sound-effects-8-bit-style)

Menu text assets and countdown text generated at [cooltext.com](https://cooltext.com)
