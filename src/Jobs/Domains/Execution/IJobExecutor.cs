namespace Jobs.Domains.Execution;

/// <summary>
/// ジョブ実行者のインターフェース
/// ローカルAI実行の橋渡し
/// </summary>
public interface IJobExecutor
{
    Task<bool> ExecuteAsync(string jobId, ExecutionTarget target, CancellationToken cancellationToken = default);
}
