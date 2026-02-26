namespace GlyphGulper.Services.Timer;

public interface IGameTimer
{
    /// <summary>
    /// Gets the current interval in milliseconds.
    /// </summary>
    double Interval { get; }

    /// <summary>
    /// Gets or sets whether the timer is currently running.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Gets the total elapsed time for the current cycle.
    /// </summary>
    TimeSpan Elapsed { get; }

    /// <summary>
    /// Advances the timer by the specified delta time.
    /// </summary>
    void Update(double deltaTimeMs);

    /// <summary>
    /// Changes the timer's duration and resets the current progress.
    /// </summary>
    void SetNewInterval(double newIntervalMs);

    /// <summary>
    /// Resets the current elapsed time to zero.
    /// </summary>
    void Reset();

    /// <summary>
    /// Disables the timer and resets its progress.
    /// </summary>
    void Stop();

    /// <summary>
    /// Enables the timer to continue tracking time.
    /// </summary>
    void Start();
}