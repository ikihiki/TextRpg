using CoreBackend.Domain.Scenarios;
using CoreBackend.Features.CreateScenario;
using Moq;

namespace CoreBackend.Features.Tests.Features.CreateScenario;

public class CreateScenarioTests
{
    private readonly Mock<IScenarioRepository> _mockRepository;
    private readonly CoreBackend.Features.CreateScenario.CreateScenario _useCase;

    public CreateScenarioTests()
    {
        _mockRepository = new Mock<IScenarioRepository>();
        _useCase = new CoreBackend.Features.CreateScenario.CreateScenario(_mockRepository.Object);
    }

    [Fact]
    public void Execute_WithValidRequest_CreatesScenarioInDraftStatus()
    {
        // Arrange
        var request = new CreateScenarioRequest
        {
            Title = "テストシナリオ",
            Summary = "テスト概要",
            UserId = "user-123"
        };

        Scenario? capturedScenario = null;
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Scenario>(), It.IsAny<CancellationToken>()))
            .Callback<Scenario, CancellationToken>((s, _) => capturedScenario = s)
            .Returns(Task.CompletedTask);

        // Act
        var response = _useCase.Execute(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.ScenarioId);
        Assert.NotEqual(Guid.Empty.ToString(), response.ScenarioId);
        Assert.True(response.CreatedAt <= DateTime.UtcNow);

        // Verify the scenario was created with correct values
        Assert.NotNull(capturedScenario);
        Assert.Equal(request.Title, capturedScenario.Title);
        Assert.Equal(request.Summary, capturedScenario.Summary);
        Assert.Equal(request.UserId, capturedScenario.OwnerId);
        Assert.Equal(ScenarioStatus.Draft, capturedScenario.Status);
    }

    [Fact]
    public void Execute_WithTitleOnly_CreatesScenarioSuccessfully()
    {
        // Arrange
        var request = new CreateScenarioRequest
        {
            Title = "タイトルのみ",
            Summary = null,
            UserId = "user-456"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Scenario>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = _useCase.Execute(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.ScenarioId);
        _mockRepository.Verify(r => r.AddAsync(
            It.Is<Scenario>(s => s.Title == request.Title && s.Summary == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Execute_CallsRepositoryAddAsync()
    {
        // Arrange
        var request = new CreateScenarioRequest
        {
            Title = "リポジトリ呼び出しテスト",
            Summary = "テスト",
            UserId = "user-789"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Scenario>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        _useCase.Execute(request);

        // Assert
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Scenario>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
