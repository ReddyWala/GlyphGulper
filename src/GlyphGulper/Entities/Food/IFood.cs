namespace GlyphGulper.Entities.Food;

public interface IFood
{
    int X { get; }
    int Y { get; }
    void Respawn(int playerX, int playerY, bool wasMissed);
    bool TryUpdateState();
}