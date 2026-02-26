using System.Collections.Concurrent;

using GlyphGulper.Services.Input;

namespace GlyphGulper.Services.Rendering;

public class RenderManager : IRenderManager, IDisposable
{
    private readonly BlockingCollection<Action> _drawQueue = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly IConsole _console;
    private Task? _renderTask;

    public RenderManager(IConsole console) => _console = console;

    public void RunRenderer()
    {
        // LongRunning hint tells the TaskScheduler to give this its own thread
        _renderTask = Task.Factory.StartNew(() =>
        {
            try
            {
                foreach (var action in _drawQueue.GetConsumingEnumerable(_cts.Token))
                {
                    action();
                }
            }
            catch (OperationCanceledException) { /* Clean exit */ }
        }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void SubmitDraw(int x, int y, string content)
    {
        if (_drawQueue.IsAddingCompleted) return;

        _drawQueue.Add(() =>
        {
            // Boundary checks to prevent crashing if terminal resizes
            if (x >= 0 && x < _console.WindowWidth && y >= 0 && y < _console.WindowHeight)
            {
                _console.SetCursorPosition(x, y);
                _console.Write(content);
            }
        });
    }

    public void SubmitClear(bool showCursor = false) => _drawQueue.Add(() =>
    {
        _console.CursorVisible = showCursor;
        _console.Clear();
    });

    public void Stop()
    {
        // 1. Mark the collection as finished. 
        // This makes 'GetConsumingEnumerable' return false ONLY after the queue is empty.
        _drawQueue.CompleteAdding();

        // 2. Wait for the task to finish processing remaining items.
        // We give it a reasonable timeout (e.g., 1 second)
        _renderTask?.Wait(1000);

        // 3. Cleanup
        _cts.Cancel();
    }

    public void Dispose()
    {
        _cts.Dispose();
        _drawQueue.Dispose();
    }
}