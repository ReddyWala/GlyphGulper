using System.Collections.Frozen;

using GlyphGulper.Extensions;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Entities.Food;

/// <summary>
/// Manages the state of a food item, including its type (e.g., Apple, Bread, Luxury) 
/// and the corresponding sprite.
/// </summary>
public class FoodStateManager
{
    /// <summary>
    /// Preloads the visual representations of each food state using the [Display] attribute 
    /// for efficient access during rendering.
    /// </summary>
    private static readonly FrozenDictionary<FoodState, string> Visuals = Enum.GetValues<FoodState>()
            .ToDictionary(v => v, v => v.GetDisplayName())
            .ToFrozenDictionary();

    /// <summary>
    /// Tracks the current state of the food (starting as Apples).
    /// </summary>
    public FoodState State { get; private set; } = FoodState.Apples;

    /// <summary>
    /// Provides the visual representation (sprite) of the current food state for rendering purposes.
    /// </summary>
    public string Sprite => Visuals[State];

    /// <summary>
    /// Initializes the FoodStateManager with the default state (Apples) and preloads visuals for all states.
    /// </summary>
    public FoodStateManager() { }

    /// <summary>
    /// Attempts to level up the food (e.g., Apple -> Burger).
    /// </summary>
    public bool TryUpgradeState()
    {
        var oldState = State;
        State = EnumExtensions.GetNextState(State);

        return oldState != State; // Return true if the state was upgraded, false if it was already at max
    }
}