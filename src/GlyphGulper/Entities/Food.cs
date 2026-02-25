using GlyphGulper.Engine;
using GlyphGulper.Models.Constants;

namespace GlyphGulper.Entities;

/// <summary>
/// Represents a food item in the game, including its position, state, and rendering logic.
/// </summary>
public class Food
{
    /// <summary>
    /// Random number generator used for respawning the food at random locations on the screen, 
    /// ensuring variability in gameplay.
    /// </summary>
    private static readonly Random Rnd = new Random();

    /// <summary>
    /// The maximum boundaries for the food's movement, used to prevent moving off-screen.
    /// </summary>
    private readonly int _maxWidth, _maxHeight;

    /// <summary>
    /// Manages the current state of the food (Apple, Bread, Luxury) and provides 
    /// the corresponding sprite for rendering.
    /// </summary>
    private readonly FoodStateManager _foodState = new FoodStateManager();

    /// <summary>
    /// Manages rendering operations for the food, allowing for thread-safe updates 
    /// to the console when the food moves or changes state.
    /// </summary>
    private readonly RenderManager _renderManager;

    /// <summary>
    /// The food's current X coordinate on the console grid. This is updated through the Respawn method.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// The food's current Y coordinate on the console grid. This is updated through the Respawn method.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Food class, setting up the RenderManager and defining the maximum
    /// boundaries for the food's movement. 
    /// </summary>
    /// <param name="renderManager">The RenderManager instance to use for rendering operations.</param>
    /// <param name="maxWidth">The maximum width of the game screen in characters.</param>
    /// <param name="maxHeight">The maximum height of the game screen in characters.</param>
    public Food(RenderManager renderManager, int maxWidth, int maxHeight)
    {
        _renderManager = renderManager;

        _maxWidth = maxWidth;
        _maxHeight = maxHeight;
    }

    /// <summary>
    /// Attempts to upgrade the food to the next state (e.g., Apple -> Bread -> Luxury) 
    /// and returns true if successful.
    /// </summary>
    /// <returns>True if the food changed its state, otherwise false.</returns>
    public bool TryUpdateState() => _foodState.TryUpgradeState();

    /// <summary>
    /// Respawns the food at a new random location on the screen, ensuring it 
    /// does not overlap with the player's visual.
    /// </summary>
    /// <param name="playerX">The X coordinate of the player's position.</param>
    /// <param name="playerY">The Y coordinate of the player's position.</param>
    /// <param name="shouldClearOld">Whether to clear the old food position before rendering the new one.</param>
    public void Respawn(int playerX, int playerY, bool shouldClearOld = true)
    {
        int newX, newY;
        int oldX = X, oldY = Y;
        int playerXMax = playerX + GameConstants.SpriteWidth;

        do
        {
            // Ensure food stays within window bounds
            newX = Rnd.Next(0, _maxWidth);
            newY = Rnd.Next(0, _maxHeight);
        }
        // Keep rolling if the food would overlap the player's position
        while (newY == playerY && newX >= playerX && newX <= playerXMax);

        X = newX;
        Y = newY;

        // Schedule Rendering (Using the thread-safe RenderManager)
        ScheduleRender(oldX, oldY, shouldClearOld);
    }

    /// <summary>
    /// Schedules the food to be re-rendered at its new position and clears the old position if necessary.
    /// </summary>
    /// <param name="oldX">The old X coordinate from which to clear.</param>
    /// <param name="oldY">The old Y coordinate from which to clear.</param>
    /// <param name="shouldClearOld">Whether to clear the old position before rendering the new one.</param>
    private void ScheduleRender(int oldX, int oldY, bool shouldClearOld = true)
    {
        int currentX = X;
        int currentY = Y;
        string currentSprite = _foodState.Sprite;
        string clearSpace = new string(' ', GameConstants.SpriteWidth);

        if (shouldClearOld)
        {
            _renderManager.SubmitDraw(oldX, oldY, clearSpace);
        }

        _renderManager.SubmitDraw(currentX, currentY, currentSprite);
    }
}