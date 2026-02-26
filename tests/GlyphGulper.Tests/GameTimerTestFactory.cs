using GlyphGulper.Engine;
using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;
using GlyphGulper.Tests;

using NSubstitute;

public static class GameTestFactory
{
    public static GameEngine Create(
        IFood? food = null,
        IPlayer? player = null,
        IRenderManager? render = null,
        IInputService? input = null,
        IResolutionManager? res = null,
        IGameTimerFactory? timerFactory = null)
    {
        return new GameEngine(
            food ?? Substitute.For<IFood>(),
            player ?? Substitute.For<IPlayer>(),
            render ?? Substitute.For<IRenderManager>(),
            input ?? Substitute.For<IInputService>(),
            res ?? Substitute.For<IResolutionManager>(),
            timerFactory ?? CreateDefaultTimerFactory()
        );
    }

    /// <summary>
    /// This is the method you were trying to implement. 
    /// It specifically allows injecting a custom/mock factory.
    /// </summary>
    public static GameEngine CreateWithSpecificFactory(IGameTimerFactory factory)
    {
        return Create(timerFactory: factory);
    }

    private static IGameTimerFactory CreateDefaultTimerFactory()
    {
        var factory = Substitute.For<IGameTimerFactory>();
        factory.Create(Arg.Any<double>(), Arg.Any<Action>(), Arg.Any<bool>())
               .Returns(Substitute.For<IGameTimer>());
        return factory;
    }

    private static MockTimer SetupMockTimer(IGameTimerFactory factory, double interval)
    {
        var mock = new MockTimer();
        factory.Create(interval, Arg.Any<Action>(), Arg.Any<bool>())
               .Returns(mock)
               .AndDoes(x => mock.Callback = x.Arg<Action>());
        return mock;
    }
}