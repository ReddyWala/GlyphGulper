using System.Diagnostics;
using GlyphGulper.Constants;
using GlyphGulper.Entities;
using GlyphGulper.Enums;
using GlyphGulper.Extensions;

namespace GlyphGulper.Engine;

/// <summary>
/// The GameEngine class is the core of the GlyphGulper game, responsible for initializing the game state,
/// managing the main game loop, handling user input, updating game logic, 
/// and coordinating rendering through the RenderManager. It also manages game timers for 
/// food vanishing, player mood updates, and UI refreshes.
/// </summary>
public class GameEngine
{
    /// <summary>
    /// Defines the thresholds for food upgrades and their corresponding vanishing intervals.
    /// (e.g., after eating 3 foods, set a faster vanishing time).
    /// </summary>
    private static readonly (int eatenFoods, int intervalMs)[] foodUpgradeThresholds = [
        (0, 9000),
        (3, 7000),
        (5, 5000),
        (10, 4000)
    ];

    /// <summary>
    /// The Player that can move around the screen and has a mood state that changes based on performance.
    /// </summary>
    private readonly Player _player;

    /// <summary>
    /// The Food that randomly spawns on the screen and can be "eaten" by the player. 
    /// It has different states (Apple, Bread, Luxury) that upgrade as the player eats more food.
    /// </summary>
    private readonly Food _food;

    // --- Game Timers ---

    /// <summary>
    /// The timer to manage the food vanishing, player mood updates.
    /// </summary>
    private readonly System.Timers.Timer _foodTimer;

    /// <summary>
    /// The timer to manage the player mood updates.
    /// </summary>
    private readonly System.Timers.Timer _moodTimer;

    /// <summary>
    /// The timer to manage the UI refreshes (e.g., score and time updates at the bottom of the screen).
    /// </summary>
    private readonly System.Timers.Timer _uiTimer;

    /// <summary>
    /// A stopwatch to track the elapsed game time, which is displayed in the UI.
    /// </summary>
    private readonly Stopwatch _gameClock = new Stopwatch();

    /// <summary>
    /// The counter to track how many foods the player has eaten.
    /// </summary>
    private int _foodEaten;

    /// <summary>
    /// The counter to track how many foods the player has missed 
    /// (i.e., how many times the food vanished before being eaten).
    /// </summary>
    private int _foodMissed;

    /// <summary>
    /// The main game loop control variable. When set to false, the game will exit.
    /// </summary>
    private bool _isRunning = true;

    /// <summary>
    /// The final result of the game, which can be Win, Loss, or Pending. 
    /// It is used to display the appropriate end screen message.
    /// </summary>
    private GameResult _result = GameResult.Pending;

    /// <summary>
    /// The screen boundaries are calculated at runtime, 
    /// we subtract a margin to prevent issues with the UI and player wrapping.
    /// </summary>
    private readonly int _maxWidth, _maxHeight;

    /// <summary>
    /// The RenderManager is responsible for handling all drawing operations 
    /// on the console in a thread-safe manner,
    /// </summary>
    private readonly RenderManager _renderManager;

    /// <summary>
    /// CancellationTokenSource to signal the RenderManager to stop when the game ends,
    /// </summary>
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// The GameEngine constructor initializes all game entities, timers, and calculates screen boundaries.
    /// </summary>
    public GameEngine()
    {
        // 1. Setup Boundaries (Subtracting safety margins for UI)
        _maxWidth = ConsoleExtensions.GetSafeWindowWidth();
        _maxHeight = ConsoleExtensions.GetSafeWindowHeight();

        // 2. Instantiate Entities
        _renderManager = new RenderManager(_cts);
        _player = new Player(_renderManager, _maxWidth, _maxHeight);
        _food = new Food(_renderManager, _maxWidth, _maxHeight);

        // 3. Setup Timers
        _foodTimer = new System.Timers.Timer(GetFoodVanishingIntervalMs()) { AutoReset = true };
        _moodTimer = new System.Timers.Timer(15000) { AutoReset = true };
        _uiTimer = new System.Timers.Timer(1000) { AutoReset = true }; // Create a 1-second "Heartbeat" for the UI
    }

    /// <summary>
    /// Starts the main game loop, initializes the console, launches the background renderer, 
    /// and handles user input and game logic until the game ends.
    /// </summary>
    public void Start()
    {
        InitializeConsole();

        // Launch Background Renderer Task
        Task renderTask = _renderManager.RunRenderer();

        // Note: The Player/Food classes now enqueue their own draw calls inside their Move/Respawn logic
        _food.Respawn(_player.X, _player.Y, false);
        _player.RenderPlayer();

        StartTimers();

        // Main Input Loop
        while (_isRunning)
        {
            if (Console.KeyAvailable)
            {
                HandleInput();
            }

            if (CheckTerminalResize()) _isRunning = false;

            Thread.Sleep(10); // Limit CPU usage
        }

        Cleanup(renderTask);
    }

    /// <summary>
    /// Handles user input for player movement and game exit.
    /// </summary>
    private void HandleInput()
    {
        var key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.UpArrow: _player.MoveUp(); break;
            case ConsoleKey.DownArrow: _player.MoveDown(); break;
            case ConsoleKey.LeftArrow: _player.MoveLeft(); break;
            case ConsoleKey.RightArrow: _player.MoveRight(); break;
            case ConsoleKey.Escape: _isRunning = false; return;
        }

        CheckCollisions();
    }

    /// <summary>
    /// Checks for collisions between the player and the food. If a collision is detected 
    /// (i.e., the player "eats" the food), it updates the score, attempts to upgrade the food,
    /// respawns the food, and resets the food vanishing timer.
    /// </summary>
    private void CheckCollisions()
    {
        // Check if player's head/sprite overlaps the food that means the player has "eaten" the food
        if (_player.Y == _food.Y && _player.X == _food.X)
        {
            _foodTimer.Stop(); // Reset the vanishing clock

            // update the counters
            _foodEaten++;
            _foodMissed = Math.Max(0, _foodMissed - 1);

            // Check if we need to upgrade the food type or vanishing speed 
            // based on the number of foods eaten
            HandleFoodUpgrade();

            // Respawn the food at a new location, ensuring it doesn't spawn on the player
            _food.Respawn(_player.X, _player.Y, false);
            // Restart the vanishing clock with the new interval if it was upgraded
            _foodTimer.Start();
        }
    }

    /// <summary>
    /// Checks if the player has reached any of the food count thresholds.
    /// </summary>
    private void HandleFoodUpgrade()
    {
        if (ReachedFoodCountTreshold())
        {
            _food.TryUpdateState(); // Attempt to upgrade the food type (e.g., Apple -> Bread)
            _foodTimer.Interval = GetFoodVanishingIntervalMs(); // Speed up food vanishing
        }
    }

    /// <summary>
    /// Starts the timers for food vanishing, player mood updates, and UI refreshes.
    /// </summary>
    private void StartTimers()
    {
        // Food Vanishing Logic
        _foodTimer.Elapsed += (s, e) =>
        {
            _foodMissed++;
            _food.Respawn(_player.X, _player.Y, true);
        };

        // Player Mood / Win-Loss Condition Logic
        _moodTimer.Elapsed += (s, e) =>
        {
            UpdatePlayerMood();
            CheckGameConditions();
        };

        _uiTimer.Elapsed += (s, e) =>
            _renderManager.SubmitDraw(0, Console.WindowHeight - 1, GetScoresLine());

        _foodTimer.Start();
        _moodTimer.Start();
        _uiTimer.Start();

        // Start the actual stopwatch when the game begins
        _gameClock.Start();
    }

    /// <summary>
    /// Helper to get the current vanishing interval based on how many foods have been eaten.
    /// </summary>
    /// <returns></returns>
    private int GetFoodVanishingIntervalMs() => foodUpgradeThresholds.FirstOrDefault(
        t => _foodEaten <= t.eatenFoods, foodUpgradeThresholds.Last()
    ).intervalMs;

    /// <summary>
    /// Checks if the player has reached any of the food count thresholds 
    /// for upgrading the food type and food vanishing timer.
    /// </summary>
    /// <returns></returns>
    private bool ReachedFoodCountTreshold() => foodUpgradeThresholds.Any(t => t.eatenFoods == _foodEaten);

    /// <summary>
    /// Updates the player's mood state based on the number of missed foods.
    /// </summary>
    private void UpdatePlayerMood()
    {
        PlayerState newState;
        newState = _foodMissed < 3 ? PlayerState.Happy :
                   _foodMissed < 6 ? PlayerState.Neutral : PlayerState.Dead;

        if (newState != _player.GetState())
        {
            _player.SetState(newState);
            _player.RenderPlayer(); // Re-render player to update the mood sprite
        }
    }

    /// <summary>
    /// Checks the win/loss conditions based on the player's performance (foods eaten vs. missed) 
    /// and updates the game state accordingly.
    /// </summary>
    private void CheckGameConditions()
    {
        if (_player.GetState() == PlayerState.Dead && _foodMissed >= 10)
        {
            _result = GameResult.Loss;
            _isRunning = false;
        }
        else if (_foodEaten >= 20 && _foodMissed == 0)
        {
            _result = GameResult.Win;
            _isRunning = false;
        }
    }

    /// <summary>
    /// Generates the score line to be displayed at the bottom of the screen, 
    /// showing the number of foods eaten, missed, and the elapsed game time.
    /// <returns></returns>
    private string GetScoresLine()
    {
        string leftSide = $" EATEN: {_foodEaten}";
        string rightSide = $"MISSED: {_foodMissed} (FOODTIMER: {_foodTimer.Interval / 1000}s)";
        string center = $"[ {_gameClock.Elapsed:mm\\:ss} ]";

        // Calculate spacing to put the clock in the middle
        int space = (Console.WindowWidth - leftSide.Length - rightSide.Length - center.Length) / 2;
        string fullLine = leftSide + new string(' ', space) + center + new string(' ', space) + rightSide;
        return fullLine;
    }

    /// <summary>
    /// Initializes the console settings for the game, such as hiding the cursor and clearing the screen.
    /// This is called once at the start of the game to set up a clean playing environment.
    /// Note: We do not set fixed window sizes to allow for dynamic resizing, but we do calculate safe boundaries.
    /// </summary>
    private void InitializeConsole()
    {
        Console.CursorVisible = false;
        Console.Clear();
    }

    /// <summary>
    /// Checks if the console window has been resized by comparing the current 
    /// safe window dimensions to the stored max width and height. If a resize is detected, 
    /// it returns true to prevent rendering issues and maintain a consistent game experience.
    /// </summary>
    private bool CheckTerminalResize() =>
        _maxHeight != ConsoleExtensions.GetSafeWindowHeight() ||
        _maxWidth != ConsoleExtensions.GetSafeWindowWidth();

    /// <summary>
    /// Cleans up resources when the game ends, such as stopping timers, canceling the renderer, 
    /// and displaying the appropriate end screen message based on the game result (win/loss).
    /// </summary>
    /// <param name="renderTask">The task responsible for rendering the game UI.</param>
    private void Cleanup(Task renderTask)
    {
        _foodTimer.Stop();
        _moodTimer.Stop();
        _cts.Cancel();

        renderTask.Wait(500); // Give the renderer a moment to finish

        Console.ResetColor();
        Console.Clear();

        if (_result == GameResult.Win)
            Console.WriteLine(GameConstants.YouWinMessage);
        else if (_result == GameResult.Loss)
            Console.WriteLine(GameConstants.GameOverMessage);

        _cts.Dispose();
    }
}