using System.Collections.Concurrent;

namespace GlyphGulper.Engine;

/// <summary>
/// Manages rendering tasks for the game. It uses a thread-safe queue to 
/// allow different parts of the game to submit drawing tasks, which are 
/// then executed by a dedicated rendering thread. This design ensures 
/// that rendering operations do not block the main game logic and allows 
/// for smooth updates to the console display.
/// </summary>
public class RenderManager
{
    /// <summary>
    /// A thread-safe queue that holds drawing tasks submitted by various parts of the game.
    /// </summary>
    private static readonly ConcurrentQueue<Action> _drawCalls = new();

    /// <summary>
    /// A cancellation token source used to signal the rendering thread to stop when the game ends.
    /// </summary>
    private readonly CancellationTokenSource cts;

    /// <summary>
    /// Initializes a new instance of the RenderManager class with the provided cancellation token source.
    /// </summary>
    /// <param name="cancellationTokenSource"></param>
    public RenderManager(CancellationTokenSource cancellationTokenSource)
    {
        cts = cancellationTokenSource;
    }

    /// <summary>
    /// Starts the rendering loop in a separate task. The loop continuously checks 
    /// for new drawing tasks in the queue and executes them until a cancellation 
    /// is requested. If the queue is empty, it yields the CPU to prevent high CPU usage. 
    /// This method should be called at the start of the game to begin processing rendering tasks.
    /// </summary>
    /// <returns>The task that represents the rendering loop.</returns>
    public Task RunRenderer()
    {
        Task renderTask = Task.Run(() =>
        {
            // The task checks the token every loop
            while (!cts.Token.IsCancellationRequested)
            {
                if (_drawCalls.TryDequeue(out var action))
                {
                    action?.Invoke();
                }
                else
                {
                    // If the queue is empty, yield the CPU so we don't 100% a core
                    //Thread.Sleep(10); 
                }
            }
        }, cts.Token);

        return renderTask;
    }

    /// <summary>
    /// Submits a drawing task to the queue. The task consists of setting the console cursor 
    /// to the specified X and Y coordinates and writing the provided content. 
    /// This method can be called from any thread and is thread-safe.
    /// </summary>
    /// <param name="X">The X coordinate where the drawing task should be executed.</param>
    /// <param name="Y">The Y coordinate where the drawing task should be executed.</param>
    /// <param name="content">The content to be written at the specified coordinates.</param>
    public void SubmitDraw(int X, int Y, string content) => _drawCalls.Enqueue(() =>
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(content);
    });
}