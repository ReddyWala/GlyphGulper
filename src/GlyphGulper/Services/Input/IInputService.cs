namespace GlyphGulper.Services.Input;

/// <summary>
/// Defines the contract for an input service that abstracts away the details of how input is received.
/// </summary>
public interface IInputService
{
    bool AnyKeysPending();
    ConsoleKey GetNextKey();
}