namespace AIOrchestrator.Domain.Providers;

/// <summary>
/// プロバイダールーターのインターフェース
/// 高品質はクラウド、機密・実験的な内容は自宅PCへルーティング
/// </summary>
public interface IProviderRouter
{
    Task<AIProvider> SelectProviderAsync(ProviderCapability requiredCapability, CancellationToken cancellationToken = default);
    Task<IEnumerable<AIProvider>> GetAvailableProvidersAsync(CancellationToken cancellationToken = default);
}
