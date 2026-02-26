using GlyphGulper.Engine;
using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Models.Constants;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;

using NSubstitute;

public static class GameEngineTestFactory
{
    public static IGameEngine CreateEngineWithMocks(
        out IFood mockFood,
        out IPlayer mockPlayer,
        out IRenderManager mockRender,
        out IInputService mockInputService,
        out IResolutionManager mockResolutionManager,
        out IGameTimerFactory mockTimerFactory)
    {
        // 1. Setup Mocks
        mockFood = Substitute.For<IFood>();
        mockPlayer = Substitute.For<IPlayer>();
        mockRender = Substitute.For<IRenderManager>();
        mockInputService = Substitute.For<IInputService>();
        mockResolutionManager = Substitute.For<IResolutionManager>();
        mockTimerFactory = Substitute.For<IGameTimerFactory>();

        // 2. Setup Default Mock Behavior (Crucial for constructor logic)
        mockResolutionManager.SafeWidth.Returns(100 - GameConstants.SpriteWidth);
        mockResolutionManager.Width.Returns(100);
        mockResolutionManager.SafeHeight.Returns(49);
        mockResolutionManager.LastLineHeight.Returns(49);
        mockPlayer.X.Returns(10);
        mockPlayer.Y.Returns(10);

        // 3. Return the Engine
        return new GameEngine(
            mockFood,
            mockPlayer,
            mockRender,
            mockInputService,
            mockResolutionManager,
            mockTimerFactory);
    }
}