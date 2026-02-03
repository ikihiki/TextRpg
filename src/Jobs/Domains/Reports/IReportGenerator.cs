namespace Jobs.Domains.Reports;

/// <summary>
/// レポート生成器のインターフェース
/// 記録物（レポート/小説）生成
/// </summary>
public interface IReportGenerator
{
    Task<string> GenerateSessionReportAsync(ReportRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateNovelExportAsync(ReportRequest request, CancellationToken cancellationToken = default);
}
