namespace GlyphGulper.Models.Constants;

/// <summary>
/// Holds constant values used throughout the game, such as sprite dimensions and endgame messages.
/// </summary>
public static class GameConstants
{
    /// <summary>
    /// All sprites are 5 characters wide, so we can use this constant for 
    /// boundary calculations and optimizations.
    /// </summary>
    public const int SpriteWidth = 5;

    /// <summary>
    /// The interval in milliseconds at which the player's mood updates, causing the sprite to change.
    /// </summary>
    public const int CheckGameConditionsIntervalMs = 15000;
    public const int EatenFoodCountToWin = 20;
    public const int MissedFoodCountToWin = 0;
    public const int MissedFoodCountToLose = 8;

    /// <summary>
    /// The message displayed on the console when the player wins the game.
    /// </summary>
    public const string YouWinMessage = @"
    __     ______  _    _  __          _______ _   _ _ 
    \ \   / / __ \| |  | | \ \        / /_   _| \ | | |
     \ \_/ / |  | | |  | |  \ \  /\  / /  | | |  \| | |
      \   /| |  | | |  | |   \ \/  \/ /   | | | . ` | |
       | | | |__| | |__| |    \  /\  /   _| |_| |\  |_|
       |_|  \____/ \____/      \/  \/   |_____|_| \_(_)
       ";

    /// <summary>
    /// The message displayed on the console when the player loses the game.
    /// </summary>
    public const string GameOverMessage = @"
     _____           __  __ ______    ______      ________ _____  
    / ____|    /\   |  \/  |  ____|  / __ \ \    / /  ____|  __ \ 
    | |  __   /  \  | \  / | |__    | |  | \ \  / /| |__  | |__) |
    | | |_ | / /\ \ | |\/| |  __|   | |  | |\ \/ / |  __| |  _  / 
    | |__| |/ ____ \| |  | | |____  | |__| | \  /  | |____| | \ \ 
    \_____ /_/    \_\_|  |_|______|  \____/   \/   |______|_|  \_\
    ";
}