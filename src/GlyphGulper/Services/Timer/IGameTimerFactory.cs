namespace GlyphGulper.Services.Timer;

public interface IGameTimerFactory
{
    IGameTimer Create(double intervalMs, Action onElapsed, bool autoReset = true);
}