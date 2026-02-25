using GlyphGulper.Enums;
using GlyphGulper.Extensions;

namespace GlyphGulper.Entities;

/// <summary>
/// Manages the state of the player, including their mood (Happy, Neutral, Dead) 
/// and the corresponding sprite.
/// </summary>
class PlayerStateManager
{
    /// <summary>
    /// Preloads the visual representations of each player state using the [Display] attribute 
    /// for efficient access during rendering.
    /// </summary>
    private readonly string[] Visuals = Enum.GetValues<PlayerState>().Select(v => v.GetDisplayName()).ToArray();

    /// <summary>
    /// Tracks the current state of the player (starting as Happy).
    /// </summary>
    public PlayerState State { get; set; } = PlayerState.Happy;

    /// <summary>
    /// Provides the visual representation (sprite) of the current player state for rendering purposes.
    /// </summary>
    public string Sprite => Visuals[(int)State];

    /// <summary>
    /// Initializes the PlayerStateManager with the default state (Happy) and preloads visuals for all states.
    /// </summary>
    public PlayerStateManager() { }
}