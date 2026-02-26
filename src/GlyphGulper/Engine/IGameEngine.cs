using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Engine;

/// <summary>
/// The GameEngine class is the core of the GlyphGulper game, responsible for initializing the game state,
/// managing the main game loop, handling user input, updating game logic, 
/// and coordinating rendering through the RenderManager. It also manages game timers for 
/// food vanishing, player mood updates, and UI refreshes.
/// </summary>
public interface IGameEngine
{
    /// <summary>
    /// The counter to track how many foods the player has eaten.
    /// </summary>
    int FoodEaten { get; }

    /// <summary>
    /// The counter to track how many foods the player has missed 
    /// (i.e., how many times the food vanished before being eaten).
    /// </summary>
    int FoodMissed { get; }

    /// <summary>
    /// The Player that can move around the screen and has a mood state that changes based on performance.
    /// </summary>
    IPlayer Player { get; }

    /// <summary>
    /// The Food that randomly spawns on the screen and can be "eaten" by the player. 
    /// It has different states (Apple, Bread, Luxury) that upgrade as the player eats more food.
    /// </summary>
    IFood Food { get; }

    /// <summary>
    /// The main game loop control variable. When set to false, the game will exit.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// The final result of the game, which can be Win, Loss, or Pending. 
    /// It is used to display the appropriate end screen message.
    /// </summary>
    GameResult Result { get; }

    /// <summary>
    /// Starts the main game loop, initializes the console, launches the background renderer, 
    /// and handles user input and game logic until the game ends.
    /// </summary>
    void Start();

    /// <summary>
    /// Updates all game timers with the elapsed time since the last frame.
    /// </summary>
    /// <param name="dt">The elapsed time in milliseconds since the last frame update.</param>
    void UpdateTimers(double dt);

    /// <summary>
    /// Checks if the player has met the winning conditions 
    /// (e.g., eating a certain number of foods without missing any).
    /// </summary>
    bool WonGame();

    /// <summary>
    /// Checks if the player has met the losing conditions.
    /// </summary>
    bool LostGame();

    /// <summary>
    /// Checks for collisions between the player and the food. If a collision is detected 
    /// (i.e., the player "eats" the food), it updates the score, attempts to upgrade the food,
    /// respawns the food, and resets the food vanishing timer.
    /// </summary>
    void CheckCollisions();
}