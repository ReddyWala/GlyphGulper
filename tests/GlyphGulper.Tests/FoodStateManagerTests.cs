using GlyphGulper.Entities.Food;
using GlyphGulper.Models.Enums;

namespace GlyphGulper.Tests;

public class FoodStateManagerTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithApples()
    {
        // Arrange & Act
        var manager = new FoodStateManager();

        // Assert
        Assert.Equal(FoodState.Apples, manager.State);
    }

    [Fact]
    public void TryUpgradeState_ShouldReturnTrue_WhenUpgradingFromApples()
    {
        // Arrange
        var manager = new FoodStateManager();

        // Act
        bool wasUpgraded = manager.TryUpgradeState();

        // Assert
        Assert.True(wasUpgraded);
        Assert.NotEqual(FoodState.Apples, manager.State);
    }

    [Theory]
    [InlineData(FoodState.Apples)]
    [InlineData(FoodState.Bread)]
    [InlineData(FoodState.Luxury)]
    public void Sprite_ShouldNotBeNullOrEmpty(FoodState state)
    {
        // Arrange
        var manager = new FoodStateManager();

        while (manager.State != state)
        {
            manager.TryUpgradeState();
        }

        // Act
        string sprite = manager.Sprite;

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(sprite));
    }

    [Fact]
    public void TryUpgradeState_ShouldReturnFalse_WhenAtMaxState()
    {
        // Arrange
        var manager = new FoodStateManager();

        // Act: Upgrade until we hit the ceiling
        // (Assuming Luxury is the last state in your Enum)
        while (manager.TryUpgradeState()) { }

        bool canUpgradeFurther = manager.TryUpgradeState();

        // Assert
        Assert.False(canUpgradeFurther);
    }
}