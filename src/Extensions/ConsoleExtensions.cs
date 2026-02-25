using GlyphGulper.Constants;

namespace GlyphGulper.Extensions;

/// <summary>
/// Provides extension methods for the Console class to calculate safe dimensions for rendering sprites,
/// ensuring that the game content does not overflow the console window and remains fully visible to 
/// the player.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Returns the maximum safe width of the console window for rendering sprites, 
    /// accounting for the sprite width to prevent overflow.
    /// </summary>F
    /// <returns>The maximum allowed width in columns.</returns>
    public static int GetSafeWindowWidth() => Console.WindowWidth - GameConstants.SpriteWidth;

    /// <summary>
    /// Returns the maximum safe height of the console window for rendering sprites, 
    /// accounting for the score line at the bottom to prevent overflow.
    /// </summary>
    /// <returns>The maximum allowed height in rows.</returns>
    public static int GetSafeWindowHeight() => Console.WindowHeight - 1; // -1 to account for the score line at the bottom
}