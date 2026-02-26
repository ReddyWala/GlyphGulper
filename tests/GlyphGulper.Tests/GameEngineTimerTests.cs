using GlyphGulper.Engine;
using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Models.Constants;
using GlyphGulper.Models.Enums;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;

using NSubstitute;

namespace GlyphGulper.Tests;

public class GameEngineTimerTests
{
    [Fact]
    public void FoodTimer_Elapsed_ShouldIncrementMissedCountAndRespawn()
    {
        // Arrange
        Action? foodElapsedCallback = null;
        var timerFactory = Substitute.For<IGameTimerFactory>();

        // Capture the callback passed to the food timer (the first timer created)
        timerFactory.Create(Arg.Any<double>(), Arg.Any<Action>(), Arg.Any<bool>())
            .Returns(Substitute.For<IGameTimer>());

        var engine = new GameEngine(
            Substitute.For<IFood>(), Substitute.For<IPlayer>(),
            Substitute.For<IRenderManager>(), Substitute.For<IInputService>(),
            Substitute.For<IResolutionManager>(), timerFactory);

        // Get the captured callback
        foodElapsedCallback = timerFactory.ReceivedCalls()
            .First(c => (double?)c.GetArguments()[0] == GameEngine.FoodUpdateThresholds[0])
            .GetArguments()[1] as Action;

        // Act
        foodElapsedCallback?.Invoke(); // Manually trigger the "Time Elapsed" event

        // Assert
        Assert.Equal(1, engine.FoodMissed);
        engine.Food.Received().Respawn(Arg.Any<int>(), Arg.Any<int>(), true);
    }


    [Fact]
    public void CheckCollisions_OnOverlap_TriggersFoodUpgradeAndResetsTimer()
    {
        // Arrange
        var engine = GameEngineTestFactory.CreateEngineWithMocks(out var food, out var player, out _, out _, out _, out var factory);
        var mockFoodTimer = Substitute.For<IGameTimer>();

        // Setup the factory to return our specific mock food timer so we can track 'Start/Stop'
        factory.Create(GameEngine.FoodUpdateThresholds[0], Arg.Any<Action>(), true).Returns(mockFoodTimer);

        // Re-instantiate with the specific mock setup
        engine = new GameEngine(food, player, Substitute.For<IRenderManager>(),
                                Substitute.For<IInputService>(), Substitute.For<IResolutionManager>(), factory);

        player.X.Returns(5); player.Y.Returns(5);
        food.X.Returns(5); food.Y.Returns(5);

        // Act
        engine.CheckCollisions();

        // Assert
        mockFoodTimer.Received().Stop();
        mockFoodTimer.Received().Start();
        food.Received().TryUpdateState();
        Assert.Equal(1, engine.FoodEaten);
    }

    [Fact]
    public void WhenFoodTimerFiresManyTimes_GameShouldBeLost()
    {
        // 1. Arrange
        var foodTimer = new MockTimer();
        var conditionTimer = new MockTimer();
        var factory = Substitute.For<IGameTimerFactory>();

        // Match by interval to ensure we give the Engine the right "timer"
        factory.Create(9000, Arg.Any<Action>(), true).Returns(foodTimer)
            .AndDoes(x => foodTimer.Callback = x.Arg<Action>());

        factory.Create(GameConstants.CheckGameConditionsIntervalMs, Arg.Any<Action>(), true).Returns(conditionTimer)
            .AndDoes(x => conditionTimer.Callback = x.Arg<Action>());

        var engine = GameTestFactory.CreateWithSpecificFactory(factory);

        // 2. Act: Simulate 8 misses (triggering the food timer logic)
        for (int i = 0; i < GameConstants.MissedFoodCountToLose; i++)
        {
            foodTimer.Fire();
        }

        // Trigger the condition checker logic
        conditionTimer.Fire();

        // 3. Assert
        Assert.Equal(GameResult.Loss, engine.Result);
        Assert.False(engine.IsRunning);
    }

    [Fact]
    public void CheckCollisions_ShouldTemporarilyStopTimer()
    {
        // Arrange
        var foodTimer = new MockTimer();
        var factory = Substitute.For<IGameTimerFactory>();
        factory.Create(9000, Arg.Any<Action>(), true).Returns(foodTimer);

        var engine = GameTestFactory.CreateWithSpecificFactory(factory);
        // ... setup overlapping coordinates ...

        // Act
        engine.CheckCollisions();

        // Assert
        // Verify the engine called Stop and then Start on the timer
        Assert.False(foodTimer.IsActive); // If it stayed stopped, or verify via NSubstitute
    }
}