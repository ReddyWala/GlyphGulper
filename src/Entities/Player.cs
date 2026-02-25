using GlyphGulper.Engine;
using GlyphGulper.Models.Constants;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Entities;

/// <summary>
/// Represents the player character, including their position, state, and rendering logic.
/// </summary>
public class Player
{
    /// <summary>
    /// The maximum boundaries for the player's movement, used to prevent moving off-screen.
    /// </summary>
    private readonly int _maxWidth, _maxHeight;

    /// <summary>
    /// Manages rendering operations for the player, allowing for thread-safe updates to the console.
    /// </summary>
    private readonly RenderManager _renderManager;

    /// <summary>
    /// Manages the player's current state (Happy, Neutral, Dead) and provides 
    /// the corresponding sprite for rendering.
    /// </summary>
    private readonly PlayerStateManager _playerState = new();

    /// <summary>
    /// The player's current X coordinate on the console grid. This is updated through movement methods.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// The player's current Y coordinate on the console grid. This is updated through movement methods.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Player class, setting the initial position 
    /// to the center of the screen and preparing the RenderManager and PlayerStateManager for use.
    /// </summary>
    /// <param name="renderManager">The RenderManager instance to use for rendering operations.</param>
    /// <param name="screenWidth">The width of the game screen in characters.</param>
    /// <param name="screenHeight">The height of the game screen in characters.</param>
    public Player(RenderManager renderManager, int screenWidth, int screenHeight)
    {
        _renderManager = renderManager;

        _maxWidth = screenWidth;
        _maxHeight = screenHeight;

        // Start player in the middle of the screen
        X = screenWidth / 2;
        Y = screenHeight / 2;
    }

    /// <summary>
    /// Sets the player's state to a new value, which will affect the sprite used for rendering.
    /// </summary>
    /// <param name="newState">The new state to set for the player.</param>
    public void SetState(PlayerState newState) => _playerState.State = newState;

    /// <summary>
    /// Retrieves the current state of the player, which can be used for game logic decisions 
    /// or rendering purposes.
    /// </summary>
    /// <returns></returns>
    public PlayerState GetState() => _playerState.State;

    /// <summary>
    /// Schedules the player to be rendered at their current position with the sprite 
    /// corresponding to their current state.
    /// </summary>
    public void RenderPlayer() => _renderManager.SubmitDraw(X, Y, _playerState.Sprite);

    // --- Directional methods for cleaner API usage ---

    /// <summary>
    /// Moves the player one unit to the right, applying boundary checks and scheduling a render update.
    /// </summary>
    public void MoveRight() => ApplyMovement(X + 1, Y);

    /// <summary>
    /// Moves the player one unit to the left, applying boundary checks and scheduling a render update.
    /// </summary>
    public void MoveLeft() => ApplyMovement(X - 1, Y);

    /// <summary>
    /// Moves the player one unit up, applying boundary checks and scheduling a render update.
    /// </summary>
    public void MoveUp() => ApplyMovement(X, Y - 1);

    /// <summary>
    /// Moves the player one unit down, applying boundary checks and scheduling a render update.
    /// </summary>
    public void MoveDown() => ApplyMovement(X, Y + 1);

    /// <summary>
    /// Applies movement to the player by calculating the new position, ensuring it stays within bounds,
    /// and scheduling a render update if the position has changed.
    /// </summary>
    /// <param name="targetX">The target X coordinate to move to.</param>
    /// <param name="targetY">The target Y coordinate to move to.</param>
    private void ApplyMovement(int targetX, int targetY)
    {
        // 1. Clamp coordinates to screen bounds
        int newX = Math.Clamp(targetX, 0, _maxWidth);
        int newY = Math.Clamp(targetY, 0, _maxHeight);

        // 2. Optimization: If we didn't actually move, don't trigger a redraw
        if (newX == X && newY == Y) return;

        // 3. Capture old position for the "Clear" operation
        int oldX = X;
        int oldY = Y;

        // 4. Update internal state
        X = newX;
        Y = newY;

        // 5. Schedule Rendering (Using the thread-safe RenderManager)
        ScheduleRender(oldX, oldY);
    }

    /// <summary>
    /// Schedules the player to be re-rendered at their new position and clears the old position.
    /// </summary>
    /// <param name="oldX">The old X coordinate from which to clear.</param>
    /// <param name="oldY">The old Y coordinate from whichto clear.</param>
    private void ScheduleRender(int oldX, int oldY)
    {
        // Capture 'X', 'Y', and 'Sprite' in local variables for thread safety (Closures)
        int currentX = X;
        int currentY = Y;
        string currentSprite = _playerState.Sprite;
        string clearSpace = new string(' ', GameConstants.SpriteWidth);

        _renderManager.SubmitDraw(oldX, oldY, clearSpace);
        _renderManager.SubmitDraw(currentX, currentY, currentSprite);
    }
}