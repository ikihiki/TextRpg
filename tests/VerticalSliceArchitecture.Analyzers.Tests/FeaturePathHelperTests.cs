using VerticalSliceArchitecture.Analyzers;

namespace VerticalSliceArchitecture.Analyzers.Tests;

public class FeaturePathHelperTests
{
    [Fact]
    public void IsFeaturesProject_WithFeaturesPath_ReturnsTrue()
    {
        // Arrange
        var path = "/src/Story/Story.Features/Features/ScenarioStart/StartScenario.cs";

        // Act
        var result = FeaturePathHelper.IsFeaturesProject(path);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFeaturesProject_WithoutFeaturesPath_ReturnsFalse()
    {
        // Arrange
        var path = "/src/Story/Story.Domain/Entities/Scenario.cs";

        // Act
        var result = FeaturePathHelper.IsFeaturesProject(path);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ExtractFeatureName_WithValidPath_ReturnsFeatureName()
    {
        // Arrange
        var path = "/src/Story/Story.Features/Features/ScenarioStart/StartScenario.cs";

        // Act
        var result = FeaturePathHelper.ExtractFeatureName(path);

        // Assert
        Assert.Equal("ScenarioStart", result);
    }

    [Fact]
    public void ExtractFeatureName_WithWindowsPath_ReturnsFeatureName()
    {
        // Arrange
        var path = @"C:\src\Story\Story.Features\Features\ScenarioStart\StartScenario.cs";

        // Act
        var result = FeaturePathHelper.ExtractFeatureName(path);

        // Assert
        Assert.Equal("ScenarioStart", result);
    }

    [Fact]
    public void ExtractFeatureName_WithInvalidPath_ReturnsNull()
    {
        // Arrange
        var path = "/src/Story/Story.Domain/Entities/Scenario.cs";

        // Act
        var result = FeaturePathHelper.ExtractFeatureName(path);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExtractUnitName_WithFeaturesProject_ReturnsUnitName()
    {
        // Arrange
        var projectName = "Story.Features";

        // Act
        var result = FeaturePathHelper.ExtractUnitName(projectName);

        // Assert
        Assert.Equal("Story", result);
    }

    [Fact]
    public void ExtractUnitName_WithDomainProject_ReturnsNull()
    {
        // Arrange
        var projectName = "Story.Domain";

        // Act
        var result = FeaturePathHelper.ExtractUnitName(projectName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void BuildExpectedNamespace_WithValidInputs_ReturnsCorrectNamespace()
    {
        // Arrange
        var unitName = "Story";
        var featureName = "ScenarioStart";

        // Act
        var result = FeaturePathHelper.BuildExpectedNamespace(unitName, featureName);

        // Assert
        Assert.Equal("Story.Features.ScenarioStart", result);
    }

    [Fact]
    public void IsValidFeaturesNamespace_WithValidNamespace_ReturnsTrue()
    {
        // Arrange
        var namespaceName = "Story.Features.ScenarioStart";

        // Act
        var result = FeaturePathHelper.IsValidFeaturesNamespace(namespaceName, out var unitName, out var featureName);

        // Assert
        Assert.True(result);
        Assert.Equal("Story", unitName);
        Assert.Equal("ScenarioStart", featureName);
    }

    [Fact]
    public void IsValidFeaturesNamespace_WithInvalidNamespace_ReturnsFalse()
    {
        // Arrange
        var namespaceName = "Story.InvalidNamespace";

        // Act
        var result = FeaturePathHelper.IsValidFeaturesNamespace(namespaceName, out var unitName, out var featureName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidFeaturesNamespace_WithNestedUnit_ReturnsTrue()
    {
        // Arrange
        var namespaceName = "TextRpg.Story.Features.ScenarioStart";

        // Act
        var result = FeaturePathHelper.IsValidFeaturesNamespace(namespaceName, out var unitName, out var featureName);

        // Assert
        Assert.True(result);
        Assert.Equal("TextRpg.Story", unitName);
        Assert.Equal("ScenarioStart", featureName);
    }
}
