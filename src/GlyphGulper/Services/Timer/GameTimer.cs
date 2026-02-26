namespace GlyphGulper.Services.Timer;

public class GameTimer(double intervalMs, Action onElapsed, bool autoReset = true) : IGameTimer
{
    private double _currentTime = 0;
    public TimeSpan Elapsed => TimeSpan.FromMilliseconds(_currentTime);
    public double Interval { get; private set; } = intervalMs;
    public bool IsActive { get; set; } = true;

    public void Update(double deltaTimeMs)
    {
        if (!IsActive) return;

        _currentTime += deltaTimeMs;
        if (_currentTime >= Interval)
        {
            onElapsed?.Invoke();
            _currentTime = autoReset ? 0 : _currentTime;
            if (!autoReset) IsActive = false;
        }
    }

    public void SetNewInterval(double newIntervalMs)
    {
        Interval = newIntervalMs;
        Reset();
    }

    public void Reset() => _currentTime = 0;

    public void Stop()
    {
        IsActive = false;
        Reset();
    }

    public void Start() => IsActive = true;
}