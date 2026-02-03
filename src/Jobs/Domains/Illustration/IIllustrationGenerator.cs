namespace Jobs.Domains.Illustration;

/// <summary>
/// 挿絵生成器のインターフェース
/// 一定ターン間隔 + イベント重要度で生成
/// </summary>
public interface IIllustrationGenerator
{
    Task<string> GenerateAsync(IllustrationRequest request, CancellationToken cancellationToken = default);
}
