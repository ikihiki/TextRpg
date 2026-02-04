namespace LocalGateway.Domain.LlmExecution;

/// <summary>
/// LLM実行者のインターフェース
/// 自宅PCでのLLM実行
/// </summary>
public interface ILlmExecutor
{
    Task<string> ExecuteAsync(LlmRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> ExecuteStreamingAsync(LlmRequest request, CancellationToken cancellationToken = default);
}
