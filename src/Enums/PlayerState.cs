using System.ComponentModel.DataAnnotations;

namespace GlyphGulper.Enums;

/// <summary>
/// Represents the different states of the player, each with a unique display name 
/// corresponding to its string representation.
/// </summary>
public enum PlayerState
{
    /// <summary>
    /// Happy (player is in a good state).
    /// </summary>
    [Display(Name = "(^-^)")]
    Happy = 0,

    /// <summary>
    /// Neutral (player is in a neutral state).
    /// </summary>
    [Display(Name = "('-')")]
    Neutral = 1,

    /// <summary>
    /// Dead (player is about to meet the loss condition).
    /// </summary>
    [Display(Name = "(X_X)")]
    Dead = 2
}