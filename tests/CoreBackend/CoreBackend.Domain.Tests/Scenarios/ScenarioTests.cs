using CoreBackend.Domain.Scenarios;

namespace CoreBackend.Domain.Tests.Scenarios;

public class ScenarioTests
{
    [Fact]
    public void Create_WithValidParameters_CreatesScenarioInDraftStatus()
    {
        // Arrange
        var title = "テストシナリオ";
        var summary = "これはテスト用の概要です";
        var ownerId = "user-123";

        // Act
        var scenario = Scenario.Create(title, summary, ownerId);

        // Assert
        Assert.NotEqual(Guid.Empty, scenario.Id.Value);
        Assert.Equal(title, scenario.Title);
        Assert.Equal(summary, scenario.Summary);
        Assert.Equal(ownerId, scenario.OwnerId);
        Assert.Equal(ScenarioStatus.Draft, scenario.Status);
        Assert.True(scenario.CreatedAt <= DateTime.UtcNow);
        Assert.True(scenario.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithNullSummary_CreatesScenarioSuccessfully()
    {
        // Arrange
        var title = "タイトルのみ";
        var ownerId = "user-456";

        // Act
        var scenario = Scenario.Create(title, null, ownerId);

        // Assert
        Assert.Equal(title, scenario.Title);
        Assert.Null(scenario.Summary);
        Assert.Equal(ScenarioStatus.Draft, scenario.Status);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            Scenario.Create("", "概要", "user-123"));
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            Scenario.Create("   ", "概要", "user-123"));
    }

    [Fact]
    public void Create_WithEmptyOwnerId_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            Scenario.Create("タイトル", "概要", ""));
    }

    [Fact]
    public async Task UpdateTitle_WithValidTitle_UpdatesTitle()
    {
        // Arrange
        var scenario = Scenario.Create("元のタイトル", "概要", "user-123");
        var newTitle = "新しいタイトル";
        var originalUpdatedAt = scenario.UpdatedAt;

        // Small delay to ensure time difference
        await Task.Delay(1);

        // Act
        scenario.UpdateTitle(newTitle);

        // Assert
        Assert.Equal(newTitle, scenario.Title);
        Assert.True(scenario.UpdatedAt >= originalUpdatedAt);
    }

    [Fact]
    public void UpdateSummary_WithValidSummary_UpdatesSummary()
    {
        // Arrange
        var scenario = Scenario.Create("タイトル", "元の概要", "user-123");
        var newSummary = "新しい概要";

        // Act
        scenario.UpdateSummary(newSummary);

        // Assert
        Assert.Equal(newSummary, scenario.Summary);
    }

    [Fact]
    public void UpdateSummary_WithNull_SetsSummaryToNull()
    {
        // Arrange
        var scenario = Scenario.Create("タイトル", "元の概要", "user-123");

        // Act
        scenario.UpdateSummary(null);

        // Assert
        Assert.Null(scenario.Summary);
    }

    [Fact]
    public void Publish_ChangesStatusToPublished()
    {
        // Arrange
        var scenario = Scenario.Create("タイトル", "概要", "user-123");

        // Act
        scenario.Publish();

        // Assert
        Assert.Equal(ScenarioStatus.Published, scenario.Status);
    }

    [Fact]
    public void Archive_ChangesStatusToArchived()
    {
        // Arrange
        var scenario = Scenario.Create("タイトル", "概要", "user-123");

        // Act
        scenario.Archive();

        // Assert
        Assert.Equal(ScenarioStatus.Archived, scenario.Status);
    }
}
