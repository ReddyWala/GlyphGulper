public enum PlayerStates{
    Happy = 0,
    Neutral = 1,
    Dead = 2
}

public class Player
{
    // Static visuals shared by all instances
    private static readonly string[] Avatars = { "(^-^)", "('-')", "(X_X)" };

    // Properties (Modern C# style)
    public int X { get; private set; }
    public int Y { get; private set; }
    public PlayerStates State { get; set; } = PlayerStates.Happy;
    
    // Calculated property for the current sprite
    public string Sprite => Avatars[(int)State];
    public int SpriteWidth => Sprite.Length;

    private readonly int _maxWidth;
    private readonly int _maxHeight;
    private readonly RenderManager _renderManager;

    public Player(RenderManager renderManager, int screenWidth, int screenHeight)
    {
        _renderManager = renderManager;

        _maxWidth = screenWidth;
        _maxHeight = screenHeight;
        
        // Start player in the middle of the screen
        X = screenWidth / 2;
        Y = screenHeight / 2;
    }

    // Directional methods for cleaner API usage
    public void MoveRight() => ApplyMovement(X + 1, Y);
    public void MoveLeft()  => ApplyMovement(X - 1, Y);
    public void MoveUp()    => ApplyMovement(X, Y - 1);
    public void MoveDown()  => ApplyMovement(X, Y + 1);

    /// <summary>
    /// Logic to handle coordinate validation and state updates.
    /// Note: This does NOT draw directly to Console to prevent thread collisions.
    /// </summary>
    private void ApplyMovement(int targetX, int targetY)
    {
        // 1. Clamp coordinates to screen bounds 
        // We subtract Width from maxWidth so the player doesn't wrap to the next line
        int newX = Math.Clamp(targetX, 0, _maxWidth - SpriteWidth);
        int newY = Math.Clamp(targetY, 0, _maxHeight - 1);

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

    public void RenderPlayer()
    {
        // This method can be used for an initial draw if needed, but subsequent moves will handle their own rendering
        _renderManager.SubmitDraw(X, Y, Sprite);
    }

    private void ScheduleRender(int oldX, int oldY)
    {
        // Capture 'X', 'Y', and 'Sprite' in local variables for thread safety (Closures)
        int currentX = X;
        int currentY = Y;
        string currentSprite = Sprite;
        string clearSpace = new string(' ', SpriteWidth);

        _renderManager.SubmitDraw(oldX, oldY, clearSpace);
        _renderManager.SubmitDraw(currentX, currentY, currentSprite);
    }
}