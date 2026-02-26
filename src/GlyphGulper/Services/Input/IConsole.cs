namespace GlyphGulper.Services.Input;

/// <summary>
/// A "dumb" wrapper for real console.
/// </summary>
public interface IConsole
{
    void SetCursorPosition(int x, int y);
    void Write(string message);
    void WriteLine(string message);
    void Clear();
    bool CursorVisible { get; set; }
    int WindowWidth { get; }
    int WindowHeight { get; }
    bool KeyAvailable { get; }
    ConsoleKeyInfo ReadKey(bool intercept);
}