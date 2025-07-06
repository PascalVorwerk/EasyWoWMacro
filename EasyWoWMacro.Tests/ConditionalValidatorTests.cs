using EasyWoWMacro.Core.Models;

namespace EasyWoWMacro.Tests;

public class ConditionalValidatorTests
{
    [Fact]
    public void IsValidCondition_ValidCondition_ShouldReturnTrue()
    {
        // Arrange
        var condition = new Condition { Key = "mod", Value = "shift" };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidCondition_InvalidCondition_ShouldReturnFalse()
    {
        // Arrange
        var condition = new Condition { Key = "invalidcondition" };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValidCondition_ValidConditionWithValue_ShouldReturnTrue()
    {
        // Arrange
        var condition = new Condition { Key = "form", Value = "1" };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsValidCondition_InvalidConditionValue_ShouldReturnFalse()
    {
        // Arrange
        var condition = new Condition { Key = "mod", Value = "invalid" };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValidCondition_EmptyKey_ShouldReturnFalse()
    {
        // Arrange
        var condition = new Condition { Key = "", Value = "shift" };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ValidateConditionSet_ValidSet_ShouldReturnNoErrors()
    {
        // Arrange
        var conditionSet = new ConditionSet
        {
            Conditions = new List<Condition>
            {
                new() { Key = "mod", Value = "shift" },
                new() { Key = "@focus" }
            }
        };

        // Act
        var errors = ConditionalValidator.ValidateConditionSet(conditionSet);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateConditionSet_InvalidSet_ShouldReturnErrors()
    {
        // Arrange
        var conditionSet = new ConditionSet
        {
            Conditions = new List<Condition>
            {
                new() { Key = "mod", Value = "shift" },
                new() { Key = "invalidcondition" }
            }
        };

        // Act
        var errors = ConditionalValidator.ValidateConditionSet(conditionSet);

        // Assert
        Assert.Single(errors);
        Assert.Contains("Invalid condition", errors[0]);
    }

    [Fact]
    public void ValidateConditional_ValidConditional_ShouldReturnNoErrors()
    {
        // Arrange
        var conditional = new Conditional
        {
            ConditionSets = new List<ConditionSet>
            {
                new()
                {
                    Conditions = new List<Condition>
                    {
                        new() { Key = "mod", Value = "shift" }
                    }
                },
                new()
                {
                    Conditions = new List<Condition>
                    {
                        new() { Key = "@focus" }
                    }
                }
            }
        };

        // Act
        var errors = ConditionalValidator.ValidateConditional(conditional);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void GetValidConditionalKeys_ShouldReturnKeys()
    {
        // Act
        var keys = ConditionalValidator.GetValidConditionalKeys().ToList();

        // Assert
        Assert.NotEmpty(keys);
        Assert.Contains("mod", keys);
        Assert.Contains("@focus", keys);
        Assert.Contains("combat", keys);
    }

    [Fact]
    public void GetValidValuesForKey_ValidKey_ShouldReturnValues()
    {
        // Act
        var values = ConditionalValidator.GetValidValuesForKey("mod").ToList();

        // Assert
        Assert.NotEmpty(values);
        Assert.Contains("alt", values);
        Assert.Contains("ctrl", values);
        Assert.Contains("shift", values);
    }

    [Fact]
    public void GetValidValuesForKey_InvalidKey_ShouldReturnEmpty()
    {
        // Act
        var values = ConditionalValidator.GetValidValuesForKey("invalidkey").ToList();

        // Assert
        Assert.Empty(values);
    }

    [Theory]
    [InlineData("mod", "alt")]
    [InlineData("mod", "ctrl")]
    [InlineData("mod", "shift")]
    [InlineData("form", "1")]
    [InlineData("form", "2")]
    [InlineData("stance", "1")]
    [InlineData("stance", "6")]
    [InlineData("equipped", "weapon")]
    [InlineData("equipped", "2h")]
    public void IsValidCondition_VariousValidConditions_ShouldReturnTrue(string key, string value)
    {
        // Arrange
        var condition = new Condition { Key = key, Value = value };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("mod", "invalid")]
    [InlineData("form", "invalidform")]
    [InlineData("stance", "7")]
    [InlineData("form", "7")]
    public void IsValidCondition_VariousInvalidConditions_ShouldReturnFalse(string key, string value)
    {
        // Arrange
        var condition = new Condition { Key = key, Value = value };

        // Act
        var isValid = ConditionalValidator.IsValidCondition(condition);

        // Assert
        Assert.False(isValid);
    }
} 