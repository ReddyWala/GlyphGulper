using GlyphGulper.Models.Enums;

namespace GlyphGulper.Entities.Player;

public interface IPlayer
{
    int X { get; }
    int Y { get; }
    PlayerState GetState();
    void SetState(PlayerState state);
    void MoveUp();
    void MoveDown();
    void MoveLeft();
    void MoveRight();
    void RenderPlayer();
}