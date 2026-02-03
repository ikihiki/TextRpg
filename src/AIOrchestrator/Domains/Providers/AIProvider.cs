namespace AIOrchestrator.Domains.Providers;

/// <summary>
/// AIプロバイダー（クラウド/ローカル）
/// </summary>
public class AIProvider
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProviderCapability Capability { get; set; } = new();
    public bool IsLocal { get; set; }
    public bool IsAvailable { get; set; }

    // TODO: Implement provider logic
}
