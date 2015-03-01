# Circle-Chain


The game is mostly controlled by the SpawningProtocol script which stores where all the blocks are in an array and spawns the blocks as needed.
The game begins when the spawners are created, at which point it spawns the first blocks.
There are two spawners, one for each player.

The BasicBehaviour script controlls how the blocks act. This includes reading player input and fallng consistantly. 

The blocks themselves are prefab objects stored in the Spawner's array called spawnable.
