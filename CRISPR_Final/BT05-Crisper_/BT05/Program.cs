
using BT05;
using BT05.screens;
using screens;

using var game = new BT05.GameBT05();
Game1.Instance.LoadConfiguration();
LogManager.Instance.CSVLogger(); // :H: Added for logging at the start of game
game.Run();