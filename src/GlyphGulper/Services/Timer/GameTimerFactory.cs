namespace GlyphGulper.Services.Timer;

public class GameTimerFactory : IGameTimerFactory
{
    public IGameTimer Create(double intervalMs, Action onElapsed, bool autoReset = true)
        => new GameTimer(intervalMs, onElapsed, autoReset);
}