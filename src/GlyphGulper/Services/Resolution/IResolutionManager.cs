namespace GlyphGulper.Services.Resolution;

/// <summary>
/// Defines the contract for a resolution manager that handles console resizing 
/// and provides information about the current console dimensions.
/// </summary>
public interface IResolutionManager
{
    int SafeWidth { get; }
    int SafeHeight { get; }
    int LastLineHeight { get; }
    int Width { get; }
    bool HasResized();
}