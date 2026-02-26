using FluentAssertions;

using GlyphGulper.Engine;
using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;

using NSubstitute;

namespace GlyphGulper.Tests;

public class GameEngineTests
{
    [Fact]
    public void CheckCollisions_ShouldIncrementScore_WhenPlayerIsOnFood()
    {
        // Arrange
        var engine = GameEngineTestFactory.CreateEngineWithMocks(
            out var food, out var player, out _, out _, out _, out _);

        // Force player and food to be at the same coordinates
        player.X.Returns(15);
        player.Y.Returns(15);
        food.X.Returns(15);
        food.Y.Returns(15);

        // Act
        engine.CheckCollisions();

        // Assert
        Assert.Equal(1, engine.FoodEaten);
        food.Received().Respawn(15, 15, false); // Verify Respawn was called
    }

    [Fact(Timeout = 2000)] // Safety: kill the test if it hangs
    public async Task Start_WhenEscapeKeyPressed_ShouldStopLoop()
    {
        // Arrange
        var engine = GameEngineTestFactory.CreateEngineWithMocks(
            out var food, out var player, out _, out var inputService, out _, out _);

        // First call returns true (key exists), second call returns false (buffer empty)
        inputService.AnyKeysPending().Returns(true, false);
        inputService.GetNextKey().Returns(ConsoleKey.Escape);

        // Act
        engine.Start(); // If it works, this returns. If not, it hangs forever.

        // Assert
        Assert.False(engine.IsRunning);
    }

    [Fact(Timeout = 2000)] // Safety: kill the test if it hangs
    public async Task Start_WhenConsoleResized_ShouldStopLoop()
    {
        // 1. Arrange
        var engine = GameEngineTestFactory.CreateEngineWithMocks(
            out _, out _, out _, out var inputService, out var resolutionManager, out _);

        // Setup: Simulate NO keys pending so the loop moves quickly to the resize check
        inputService.AnyKeysPending().Returns(false);

        // Setup: The FIRST time the loop checks, return true (resized).
        // This will hit the 'if (_resolutionManager.HasResized()) IsRunning = false;' line.
        resolutionManager.HasResized().Returns(true);

        // 2. Act
        engine.Start();

        // 3. Assert
        Assert.False(engine.IsRunning);
        resolutionManager.Received().HasResized(); // Verify the check actually happened
    }

    [Fact]
    public void CheckCollisions_WhenPlayerOnFood_ShouldIncreaseScoreAndRespawn()
    {
        // Arrange
        var engine = GameEngineTestFactory.CreateEngineWithMocks(out var food, out var player, out _, out _, out _, out _);
        player.X.Returns(10);
        player.Y.Returns(10);
        food.X.Returns(10);
        food.Y.Returns(10);

        // Act
        engine.CheckCollisions();

        // Assert
        engine.FoodEaten.Should().Be(1);
        food.Received().Respawn(10, 10, false);
    }

    [Fact]
    public void HandleFoodUpgrade_WhenThresholdReached_ShouldCallTryUpdateState()
    {
        // 1. Arrange: Create mocks for the engine's dependencies
        var mockFood = Substitute.For<IFood>();
        var mockPlayer = Substitute.For<IPlayer>();
        var mockRender = Substitute.For<IRenderManager>();
        var mockInput = Substitute.For<IInputService>();
        var mockRes = Substitute.For<IResolutionManager>();
        var mockTimerFactory = Substitute.For<IGameTimerFactory>();

        // 2. Instantiate the REAL engine
        var engine = new GameEngine(mockFood, mockPlayer, mockRender, mockInput, mockRes, mockTimerFactory);

        // Act
        Enumerable.Range(0, GameEngine.FoodUpdateThresholds.Keys.Skip(1).First())
            .ToList().ForEach(_ => engine.CheckCollisions());

        // Assert
        mockFood.Received(1).TryUpdateState();
    }
}