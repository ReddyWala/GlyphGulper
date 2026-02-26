using System.Collections.Frozen;

using GlyphGulper.Extensions;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Entities.Player;

/// <summary>
/// Manages the state of the player, including their mood (Happy, Neutral, Dead) 
/// and the corresponding sprite.
/// </summary>
public class PlayerStateManager
{
    /// <summary>
    /// Preloads the visual representations of each player state using the [Display] attribute 
    /// for efficient access during rendering.
    /// </summary>
    private static readonly FrozenDictionary<PlayerState, string> Visuals = Enum.GetValues<PlayerState>()
            .ToDictionary(v => v, v => v.GetDisplayName())
            .ToFrozenDictionary();

    /// <summary>
    /// Tracks the current state of the player (starting as Happy).
    /// </summary>
    public PlayerState State { get; set; } = PlayerState.Happy;

    /// <summary>
    /// Provides the visual representation (sprite) of the current player state for rendering purposes.
    /// </summary>
    public string Sprite => Visuals[State];

    /// <summary>
    /// Initializes the PlayerStateManager with the default state (Happy) and preloads visuals for all states.
    /// </summary>
    public PlayerStateManager() { }
}