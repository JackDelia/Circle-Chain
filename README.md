# Circle-Chain

Files:

All scripts controlling the game are in the scripts folder inside the assets folder.
Scripts:
AI: The script controlling the AI
BasicBehaviour: Controlls the behaviour of individual blocks
MenuScript: Controllls menu navigation
Optional: Stores the game options
RNG: The random number generator used to generate blocks
SpawningProtocol: Used to spawn blocks and generally control the game.

The Images used in game are stored under images in assets.
The sounds are stored under sounds in assets.
The prefabs for the projects(in this case the blocks themselves) are stored under prefabs in assets
The different scenes for the project are stored under scenes in assets.

Scenes:
GameOver: Used to replay once the game ends.
Menu: The starting menu, where the game starts.
Multiplayer: The multiplayer game mode.
Options: The options menu.
ScoreAttack: The score attack game mode.
SinglePlayer: The vs AI game mode.

The game is mostly controlled by the SpawningProtocol script which stores where all the blocks are in an array and spawns the blocks as needed.
The game begins when the spawners are created, at which point it spawns the first blocks.
There are two spawners, one for each player.

The BasicBehaviour script controlls how the blocks act. This includes reading player input and fallng consistantly. 

The blocks themselves are prefab objects stored in the Spawner's array called spawnable.

