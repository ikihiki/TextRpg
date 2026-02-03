namespace Jobs.Domains.Reports;

/// <summary>
/// レポートリクエスト
/// </summary>
public class ReportRequest
{
    public Guid SessionId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public int? FromTurn { get; set; }
    public int? ToTurn { get; set; }

    // TODO: Implement report request logic
}
