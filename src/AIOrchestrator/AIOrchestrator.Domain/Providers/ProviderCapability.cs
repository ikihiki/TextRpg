namespace AIOrchestrator.Domain.Providers;

/// <summary>
/// プロバイダーの能力
/// </summary>
public class ProviderCapability
{
    public bool SupportsNarrative { get; set; }
    public bool SupportsImageGeneration { get; set; }
    public int MaxContextLength { get; set; }
    public List<string> SupportedLanguages { get; set; } = new();

    // TODO: Implement capability logic
}
