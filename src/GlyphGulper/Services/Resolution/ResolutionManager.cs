using GlyphGulper.Models.Constants;
using GlyphGulper.Services.Input;

namespace GlyphGulper.Services.Resolution;

public class ResolutionManager : IResolutionManager
{
    private readonly IConsole _console;
    private int _lastWidth;
    private int _lastHeight;

    public ResolutionManager(IConsole console)
    {
        _console = console;
        _lastWidth = _console.WindowWidth;
        _lastHeight = _console.WindowHeight;
    }

    // Business Logic lives here, not in the Console wrapper
    public int SafeWidth => _console.WindowWidth - GameConstants.SpriteWidth;
    public int SafeHeight => LastLineHeight - 1;
    public int LastLineHeight => _console.WindowHeight - 1;
    public int Width => _console.WindowWidth;

    public bool HasResized()
    {
        bool changed = _console.WindowWidth != _lastWidth || _console.WindowHeight != _lastHeight;
        if (changed)
        {
            _lastWidth = _console.WindowWidth;
            _lastHeight = _console.WindowHeight;
        }
        return changed;
    }
}