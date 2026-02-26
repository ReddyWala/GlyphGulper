namespace GlyphGulper.Services.Rendering;

/// <summary>
/// Defines the contract for a render manager that handles drawing operations and manages the rendering loop.
/// </summary>
public interface IRenderManager
{
    void RunRenderer();
    void SubmitDraw(int x, int y, string content);
    void SubmitClear(bool showCursor = false);
    void Stop();
}