namespace GlyphGulper.Enums;

/// <summary>
/// Defines the possible outcomes of the game.
/// </summary>
public enum GameResult
{ 
    /// <summary>
    /// Pending (game is still in progress).
    /// </summary>
    Pending, 

    /// <summary>
    /// Win (player has met the win condition).
    /// </summary>
    Win, 

    /// <summary>
    /// Loss (player has met the loss condition).
    /// </summary>
    Loss 
}