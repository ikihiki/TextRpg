namespace LocalGateway.Infrastructure.Streaming;

/// <summary>
/// ワークストリームクライアント
/// 双方向ストリーミングでジョブ待受（アウトバウンド接続のみでNAT回避）
/// </summary>
public class WorkStreamClient
{
    private readonly string _serverUrl;

    public WorkStreamClient(string serverUrl)
    {
        _serverUrl = serverUrl;
    }

    public async Task ConnectAndListenAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Implement bidirectional gRPC streaming
        // Local Gateway が常時接続してジョブを待つ
        await Task.CompletedTask;
    }
}
