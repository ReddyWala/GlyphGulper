using System.Collections.Frozen;
using System.Diagnostics;

using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Models.Constants;
using GlyphGulper.Models.Enums;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;

namespace GlyphGulper.Engine;

/// <inheritdoc />
public class GameEngine : IGameEngine
{
    /// <summary>
    /// Defines the thresholds for food upgrades and their corresponding vanishing intervals.
    /// (e.g., after eating 3 foods, set a faster vanishing time).
    /// </summary>
    public static readonly FrozenDictionary<int, int> FoodUpdateThresholds =
    new Dictionary<int, int>(new Dictionary<int, int>
    {
        [0] = 9000,
        [3] = 7000,
        [5] = 5000,
        [10] = 4000
    }).ToFrozenDictionary();

    /// <summary>
    /// Defines the thresholds for player mood updates based on the number of missed foods,
    /// </summary>
    public static readonly FrozenDictionary<int, PlayerState> MoodUpdateThresholds =
    new Dictionary<int, PlayerState>(new Dictionary<int, PlayerState>
    {
        [2] = PlayerState.Happy,
        [4] = PlayerState.Neutral,
        [6] = PlayerState.Dead
    }).ToFrozenDictionary();

    private readonly IPlayer _player;

    /// <inheritdoc />
    public IPlayer Player => _player;

    private readonly IFood _food;

    /// <inheritdoc />
    public IFood Food => _food;

    // --- Game Timers ---
    private readonly List<IGameTimer> _timers = new();
    private readonly Stopwatch _frameClock = new();
    private double _totalElapseMilliseconds = 0;

    // The 3 Specific Timers
    private readonly IGameTimer _foodTimer;
    private readonly IGameTimer _forceEndGameTimer;
    private readonly IGameTimer _uiTimer;

    /// <summary>
    /// The RenderManager is responsible for handling all drawing operations 
    /// on the console in a thread-safe manner,
    /// </summary>
    private readonly IRenderManager _renderManager;

    /// <summary>
    /// Abstracts away console input operations, allowing for 
    /// non-blocking input handling and easier testing.
    /// </summary>
    private readonly IInputService _inputService;

    /// <summary>
    /// The IResolutionManager is responsible for managing screen resolution and boundaries.
    /// </summary>
    private readonly IResolutionManager _resolutionManager;

    /// <inheritdoc />
    public int FoodEaten { get; private set; }

    /// <inheritdoc />
    public int FoodMissed { get; private set; }

    /// <inheritdoc />
    public bool IsRunning { get; private set; } = true;

    /// <inheritdoc />
    public GameResult Result { get; private set; } = GameResult.Pending;

    /// <summary>
    /// The GameEngine constructor initializes all game entities, timers, and services.
    /// </summary>
    public GameEngine(
        IFood food,
        IPlayer player,
        IRenderManager renderManager,
        IInputService inputService,
        IResolutionManager resolutionManager,
        IGameTimerFactory timerFactory)
    {
        // Instantiate Entities
        _renderManager = renderManager;
        _player = player;
        _food = food;
        _inputService = inputService;
        _resolutionManager = resolutionManager;

        // Setup Timers
        _foodTimer = timerFactory.Create(FoodUpdateThresholds.Values.First(), () =>
        {
            FoodMissed++;
            _food.Respawn(_player.X, _player.Y, true);

            // Check if we need to update the player's mood based on the number of foods missed
            UpdatePlayerMood();
        });

        _forceEndGameTimer = timerFactory.Create(GameConstants.CheckGameConditionsIntervalMs, () =>
        {
            CheckGameConditions();
        });

        _uiTimer = timerFactory.Create(1000, () => DrawScores());

        _timers.AddRange([_foodTimer, _forceEndGameTimer, _uiTimer]);
    }

    /// <inheritdoc />
    public void Start()
    {
        InitializeConsole();

        // Launch Background Renderer Task
        _renderManager.RunRenderer();

        // Note: The Player/Food classes now enqueue their own draw calls inside their Move/Respawn logic
        _food.Respawn(_player.X, _player.Y, false);
        _player.RenderPlayer();

        _frameClock.Start();

        // Main Input Loop
        while (IsRunning)
        {
            double deltaTime = _frameClock.Elapsed.TotalMilliseconds;
            _totalElapseMilliseconds += deltaTime;
            _frameClock.Restart();

            // NON-BLOCKING INPUT
            // Only call HandleInput if there's a key to read
            // Decoupled Input Handling - no longer check _console.KeyAvailable directly
            while (_inputService.AnyKeysPending())
            {
                HandleInput();
            }

            if (_resolutionManager.HasResized()) IsRunning = false;

            UpdateTimers(deltaTime);

            Thread.Sleep(10); // Limit CPU usage
        }

        FinishGame();
    }

    /// <inheritdoc />
    public void UpdateTimers(double dt)
    {
        foreach (var timer in _timers) timer.Update(dt);
    }

    /// <inheritdoc />
    public bool WonGame() => FoodEaten >= GameConstants.EatenFoodCountToWin &&
        FoodMissed <= GameConstants.MissedFoodCountToWin;

    /// <inheritdoc />
    public bool LostGame() => FoodMissed >= GameConstants.MissedFoodCountToLose;

    /// <inheritdoc />
    public void CheckCollisions()
    {
        // Check if player's head/sprite overlaps the food that means the player has "eaten" the food
        if (_player.Y == _food.Y && _player.X == _food.X)
        {
            _foodTimer.Stop(); // Pause the vanishing clock while we handle the eating logic

            // update the counters
            FoodEaten++;
            FoodMissed = Math.Max(0, FoodMissed - 1);

            // Check if we need to upgrade the food type or vanishing speed 
            // based on the number of foods eaten
            TryHandleFoodUpgrade();

            // Respawn the food at a new location, ensuring it doesn't spawn on the player
            _food.Respawn(_player.X, _player.Y, false);
            _foodTimer.Start(); // Resume the vanishing clock after handling the eating logic
        }
    }

    /// <summary>
    /// Handles user input for player movement and game exit.
    /// </summary>
    internal void HandleInput()
    {
        var key = _inputService.GetNextKey();

        switch (key)
        {
            case ConsoleKey.UpArrow: _player.MoveUp(); break;
            case ConsoleKey.DownArrow: _player.MoveDown(); break;
            case ConsoleKey.LeftArrow: _player.MoveLeft(); break;
            case ConsoleKey.RightArrow: _player.MoveRight(); break;
            case ConsoleKey.Escape: IsRunning = false; return;
        }

        CheckCollisions();
    }

    /// <inheritdoc />
    internal bool TryHandleFoodUpgrade()
    {
        if (FoodUpdateThresholds.ContainsKey(FoodEaten))
        {
            _food.TryUpdateState(); // Attempt to upgrade the food type (e.g., Apple -> Bread)
            _foodTimer.SetNewInterval(FoodUpdateThresholds[FoodEaten]); // Update the vanishing timer based on the new threshold
            return true;
        }

        return false;
    }

    /// <summary>
    /// Updates the player's mood state based on the number of missed foods.
    /// </summary>
    internal void UpdatePlayerMood()
    {
        if (MoodUpdateThresholds.ContainsKey(FoodMissed))
        {
            _player.SetState(MoodUpdateThresholds[FoodMissed]);
            _player.RenderPlayer(); // Re-render player to update the mood sprite
        }
    }

    /// <summary>
    /// Checks the win/loss conditions based on the player's performance (foods eaten vs. missed) 
    /// and updates the game state accordingly.
    /// </summary>
    internal void CheckGameConditions()
    {
        if (WonGame())
        {
            Result = GameResult.Win;
            IsRunning = false;
        }
        else if (LostGame())
        {
            Result = GameResult.Loss;
            IsRunning = false;
        }
    }

    /// <summary>
    /// Generates the score line to be displayed at the bottom of the screen, 
    /// showing the number of foods eaten, missed, and the elapsed game time.
    internal string GetScoresLine()
    {
        string leftSide = $" EATEN: {FoodEaten}";
        string rightSide = $"MISSED: {FoodMissed} (FOODTIMER: {_foodTimer.Interval / 1000}s)";
        string center = $"[ {TimeSpan.FromMilliseconds(_totalElapseMilliseconds):mm\\:ss} ]";

        // Calculate spacing to put the clock in the middle
        int space = (_resolutionManager.Width - leftSide.Length - rightSide.Length - center.Length) / 2;
        string fullLine = leftSide + new string(' ', space) + center + new string(' ', space) + rightSide;

        return fullLine;
    }

    private void DrawScores()
    {
        _renderManager.SubmitDraw(0, _resolutionManager.LastLineHeight, GetScoresLine());
    }

    /// <summary>
    /// Initializes the console settings for the game, such as hiding the cursor and clearing the screen.
    /// This is called once at the start of the game to set up a clean playing environment.
    /// Note: We do not set fixed window sizes to allow for dynamic resizing, but we do calculate safe boundaries.
    /// </summary>
    internal void InitializeConsole() => _renderManager.SubmitClear();

    /// <summary>
    /// Cleans up resources when the game ends, such as stopping timers, canceling the renderer, 
    /// and displaying the appropriate end screen message based on the game result (win/loss).
    /// </summary>
    internal void FinishGame()
    {
        //_console.ResetColor();
        //_console.Clear();
        _renderManager.SubmitClear(true);

        if (Result == GameResult.Win)
            _renderManager.SubmitDraw(0, 0, GameConstants.YouWinMessage);
        else if (Result == GameResult.Loss)
            _renderManager.SubmitDraw(0, 0, GameConstants.GameOverMessage);

        Thread.Sleep(100);
        _renderManager.Stop();
    }
}