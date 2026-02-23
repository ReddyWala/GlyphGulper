public enum FoodStates
{
    Apples = 0,
    Bread = 1,
    Luxury = 2
}

public class Food
{
    // Static fields shared by ALL food instances (saves memory)
    private static readonly string[] Visuals = { "@@@@@", "#####", "$$$$$" };
    private static readonly Random Rnd = new Random();
    private static readonly int MaxStateValue = (int)Enum.GetValues<FoodStates>().Max();

    // Instance properties (Auto-properties are cleaner than Get/Set methods)
    public int X { get; private set; }
    public int Y { get; private set; }
    public FoodStates State { get; private set; } = FoodStates.Apples;

    public string Sprite => Visuals[(int)State];
    public int SpriteWidth => Sprite.Length;

    private int _maxWidth;
    private int _maxHeight;

    private readonly RenderManager _renderManager;

    public Food(RenderManager renderManager, int maxWidth, int maxHeight)
    {
        _renderManager = renderManager;

        _maxWidth = maxWidth;
        _maxHeight = maxHeight;
    }

    /// <summary>
    /// Attempts to level up the food (e.g., Apple -> Burger).
    /// Returns true if successful.
    /// </summary>
    public bool TryUpgradeState()
    {
        int nextValue = (int)State + 1;

        if (nextValue <= MaxStateValue)
        {
            State = (FoodStates)nextValue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculates a new valid position for the food.
    /// </summary>
    public void Respawn(int playerX, int playerY, int playerWidth, bool shouldClearOld = true)
    {
        int newX, newY;
        int oldX = X, oldY = Y;
        int foodWidth = Visuals[(int)State].Length;
        int playerXMax = playerX + playerWidth;

        do
        {
            // Ensure food stays within window bounds minus its own width
            newX = Rnd.Next(0, _maxWidth - foodWidth);
            newY = Rnd.Next(0, _maxHeight);
        } 
        // Logic: Keep rolling if the food would overlap the player's horizontal line
        while (newY == playerY && newX >= playerX && newX <= playerXMax);

        X = newX;
        Y = newY;

        // Schedule Rendering (Using the thread-safe RenderManager)
        ScheduleRender(oldX, oldY, shouldClearOld);
    }

    // Returns the visual string based on current state
    public string GetVisual() => Visuals[(int)State];

    private void ScheduleRender(int oldX, int oldY, bool shouldClearOld = true)
    {
        int currentX = X;
        int currentY = Y;
        string currentSprite = Sprite;
        string clearSpace = new string(' ', SpriteWidth);

        if (shouldClearOld)
        {
            _renderManager.SubmitDraw(oldX, oldY, clearSpace);
        }

        _renderManager.SubmitDraw(currentX, currentY, currentSprite);
    }
}