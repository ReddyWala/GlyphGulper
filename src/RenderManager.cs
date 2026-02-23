using System.Collections.Concurrent;

public class RenderManager
{
    private static readonly ConcurrentQueue<Action> _drawCalls = new();

    // Instances call this to "schedule" a drawing task
    public void SubmitDraw(int X, int Y, string content) => _drawCalls.Enqueue(() =>
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(content);
    });

    private readonly CancellationTokenSource cts;

    public RenderManager(CancellationTokenSource cancellationTokenSource)
    {
        cts = cancellationTokenSource;
    }

    // The Main thread calls this to actually paint the screen
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
}