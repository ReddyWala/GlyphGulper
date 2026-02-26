using GlyphGulper.Services.Timer;

namespace GlyphGulper.Tests;

public class MockTimer : IGameTimer
{
    // The engine's logic is stored here
    public Action Callback { get; set; }

    // IGameTimer Implementation
    public double Interval { get; private set; }
    public bool IsActive { get; set; }
    public TimeSpan Elapsed => TimeSpan.Zero;

    public void Update(double deltaTimeMs) { /* No-op in tests */ }
    public void SetNewInterval(double ms) => Interval = ms;
    public void Reset() { }
    public void Stop() => IsActive = false;
    public void Start() => IsActive = true;

    // Helper for the test to "fire" the timer logic
    public void Fire() => Callback?.Invoke();
}