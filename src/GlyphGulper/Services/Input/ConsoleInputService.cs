namespace GlyphGulper.Services.Input;

public class ConsoleInputService(IConsole console) : IInputService
{
    public bool AnyKeysPending() => console.KeyAvailable;
    public ConsoleKey GetNextKey() => console.ReadKey(true).Key;
}