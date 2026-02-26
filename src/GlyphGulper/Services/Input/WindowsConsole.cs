using GlyphGulper.Models.Constants;

namespace GlyphGulper.Services.Input;

/// <summary>
/// A solid implementation of the Adapter Pattern. By wrapping the static 
/// System.Console calls, we’ve successfully decoupled your game logic 
/// from the operating system's console.
/// </summary>
public class WindowsConsole : IConsole
{
    public void SetCursorPosition(int x, int y) => Console.SetCursorPosition(x, y);

    public void Write(string message) => Console.Write(message);

    public void WriteLine(string message) => Console.WriteLine(message);

    public void Clear() => Console.Clear();

    public bool CursorVisible
    {
        get => Console.CursorVisible;
        set => Console.CursorVisible = value;
    }

    public int SafeWindowWidth => Console.WindowWidth - GameConstants.SpriteWidth;
    public int SafeWindowHeight => Console.WindowHeight - 1;

    public bool KeyAvailable => Console.KeyAvailable;

    public int WindowWidth => Console.WindowWidth;

    public int WindowHeight => Console.WindowHeight;

    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
}