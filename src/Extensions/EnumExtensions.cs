using System.ComponentModel.DataAnnotations;
using System.Reflection;
using GlyphGulper.Enums;

namespace GlyphGulper.Extensions;

/// <summary>
/// Provides extension methods for enums, including retrieving display names and cycling through states.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Retrieves the display name of an enum value based on the [Display] attribute.
    /// </summary>
    /// <param name="enumValue">The enum value to retrieve the display name for.</param>
    /// <returns>The display name of the enum value, or the enum name if no [Display] attribute is found.</returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        var type = enumValue.GetType();
        var memberInfo = type.GetMember(enumValue.ToString());

        // Look for the DisplayAttribute
        var attribute = memberInfo.FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();

        // Return the Name if it exists, otherwise fall back to the enum name
        return attribute?.Name ?? enumValue.ToString();
    }

    /// <summary>
    /// Returns the next state in the FoodState enum, cycling through the states in a defined order.
    /// </summary>
    /// <param name="current">The current FoodState value.</param>
    /// <returns>The next FoodState value in the sequence.</returns>
    public static FoodState GetNextState(FoodState current) => current switch
    {
        FoodState.Apples => FoodState.Bread,
        FoodState.Bread => FoodState.Luxury,
        FoodState.Luxury => FoodState.Luxury,
        _ => FoodState.Apples
    };
}