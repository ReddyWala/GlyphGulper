using System.ComponentModel.DataAnnotations;

namespace GlyphGulper.Models.Enums;

/// <summary>
/// Represents the different types of food that can appear in the game, each with a unique 
/// display name corresponding to its string representation.
/// </summary>
public enum FoodState
{
    /// <summary>F
    /// Apples (basic food item, provides minimal points).
    /// </summary>
    [Display(Name = "@@@@@")]
    Apples = 0,

    /// <summary>
    /// Bread (intermediate food item, provides more points than apples).
    /// </summary>
    [Display(Name = "#####")]
    Bread = 1,

    /// <summary>
    /// Luxury (high-value food item, provides the most points and is the final upgrade).
    /// </summary>
    [Display(Name = "$$$$$")]
    Luxury = 2
}