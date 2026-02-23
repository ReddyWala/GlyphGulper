using System.Diagnostics;

public enum GameResult { Pending, Win, Loss }

public class GameEngine
{
    // Constants for Game Tuning
    private const int FoodVanishingInitialMs = 9000;
    private const int FoodSpeedUpStepMs = 2000;
    private const int MinFoodIntervalMs = 4000;
    private const int PlayerMoodIntervalMs = 15000;
    private const int WinThreshold = 20;

    // Game Entities
    private readonly Player _player;
    private readonly Food _food;
    private readonly System.Timers.Timer _foodTimer;
    private readonly System.Timers.Timer _moodTimer;
    // Create a 1-second "Heartbeat" for the UI
    private readonly System.Timers.Timer _uiTimer;

    // State Variables
    private int _foodEaten;
    private int _foodMissed;
    private int _upgradeThreshold = 3;
    private bool _isRunning = true;
    private GameResult _result = GameResult.Pending;
    private readonly Stopwatch _gameClock = new Stopwatch();
    private string ElapsedTime => _gameClock.Elapsed.ToString(@"mm\:ss");

    private readonly int _maxWidth;
    private readonly int _maxHeight;
    private readonly CancellationTokenSource _cts = new();
    private readonly RenderManager _renderManager;

    public GameEngine()
    {
        // 1. Setup Boundaries (Subtracting safety margins for UI)
        _maxWidth = Console.WindowWidth - 5;
        _maxHeight = Console.WindowHeight - 2;

        // 2. Instantiate Entities
        _renderManager = new RenderManager(_cts);
        _player = new Player(_renderManager, _maxWidth, _maxHeight);
        _food = new Food(_renderManager, _maxWidth, _maxHeight);

        // 3. Setup Timers
        _foodTimer = new System.Timers.Timer(FoodVanishingInitialMs) { AutoReset = true };
        _moodTimer = new System.Timers.Timer(PlayerMoodIntervalMs) {  AutoReset = true };
        _uiTimer = new System.Timers.Timer(1000) { AutoReset = true };
    }

    public void Start()
    {
        InitializeConsole();

        // Launch Background Renderer Task
        Task renderTask = _renderManager.RunRenderer();

        // Note: The Player/Food classes now enqueue their own draw calls inside their Move/Respawn logic
        _food.Respawn(_player.X, _player.Y, _player.SpriteWidth, false);
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

    private void CheckCollisions()
    {
        // Check if player's head/sprite overlaps the food
        if (_player.Y == _food.Y && _player.X == _food.X)
        {
            _foodTimer.Stop(); // Reset the vanishing clock

            _foodEaten++;
            _foodMissed = Math.Max(0, _foodMissed - 1);

            HandleFoodUpgrade();

            _food.Respawn(_player.X, _player.Y, _player.SpriteWidth, false);
            _foodTimer.Start();
        }
    }

    private void HandleFoodUpgrade()
    {
        if (_foodEaten >= _upgradeThreshold)
        {
            if (_food.TryUpgradeState())
            {
                _upgradeThreshold += 5;
                // Speed up the vanishing timer
                var newInterval = _foodTimer.Interval - FoodSpeedUpStepMs;
                _foodTimer.Interval = Math.Max(newInterval, MinFoodIntervalMs);
            }
        }
    }

    private void StartTimers()
    {
        // Food Vanishing Logic
        _foodTimer.Elapsed += (s, e) =>
        {
            _foodMissed++;
            _food.Respawn(_player.X, _player.Y, _player.SpriteWidth, true);
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

    private void UpdatePlayerMood()
    {
        if (_foodMissed < 3) _player.State = PlayerStates.Happy;
        else if (_foodMissed < 6) _player.State = PlayerStates.Neutral;
        else _player.State = PlayerStates.Dead;
    }

    private void CheckGameConditions()
    {
        if (_player.State == PlayerStates.Dead)
        {
            _result = GameResult.Loss;
            _isRunning = false;
        }
        else if (_foodEaten >= WinThreshold && _foodMissed == 0)
        {
            _result = GameResult.Win;
            _isRunning = false;
        }
    }

    private string GetScoresLine()
    {
        string leftSide = $" EATEN: {_foodEaten}";
        string rightSide = $"MISSED: {_foodMissed} ";
        string center = $"[ {ElapsedTime} ]";

        // Calculate spacing to put the clock in the middle
        int space = (Console.WindowWidth - leftSide.Length - rightSide.Length - center.Length) / 2;
        string fullLine = leftSide + new string(' ', space) + center + new string(' ', space) + rightSide;
        return fullLine;
    }

    private void InitializeConsole()
    {
        Console.CursorVisible = false;
        Console.Clear();
    }

    private bool CheckTerminalResize() =>
        _maxHeight != Console.WindowHeight - 2 || _maxWidth != Console.WindowWidth - 5;

    private void Cleanup(Task renderTask)
    {
        _foodTimer.Stop();
        _moodTimer.Stop();
        _cts.Cancel();

        renderTask.Wait(500); // Give the renderer a moment to finish

        Console.ResetColor();
        Console.Clear();

        if (_result == GameResult.Win)
            Console.WriteLine(Constants.YouWinMessage);
        else if (_result == GameResult.Loss)
            Console.WriteLine(Constants.GameOverMessage);

        _cts.Dispose();
    }
}