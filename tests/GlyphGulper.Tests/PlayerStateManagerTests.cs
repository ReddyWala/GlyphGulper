using FluentAssertions;

using GlyphGulper.Entities.Player;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Tests;

public class PlayerStateManagerTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultState()
    {
        // Arrange & Act
        var manager = new PlayerStateManager();

        // Assert
        manager.State.Should().Be(PlayerState.Happy);
    }

    [Theory]
    [InlineData(PlayerState.Happy, "(^-^)")]   // Assuming these are your [Display] names
    [InlineData(PlayerState.Neutral, "('-')")]
    [InlineData(PlayerState.Dead, "(X_X)")]
    public void Sprite_ShouldMatchExpectedDisplayNameForState(PlayerState state, string expectedSprite)
    {
        // Arrange
        var manager = new PlayerStateManager();

        // Act
        manager.State = state;

        // Assert
        manager.Sprite.Should().Be(expectedSprite);
    }

    [Fact]
    public void StateChange_ShouldUpdateSpriteDynamically()
    {
        // Arrange
        var manager = new PlayerStateManager();
        var initialSprite = manager.Sprite;

        // Act
        manager.State = PlayerState.Dead;

        // Assert
        manager.Sprite.Should().NotBe(initialSprite);
        manager.State.Should().Be(PlayerState.Dead);
    }
}