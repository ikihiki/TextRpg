namespace CoreBackend.Domain.Assets;

/// <summary>
/// Data transfer object for Asset information
/// </summary>
public class AssetData
{
    public Guid AssetId { get; set; }
    public Guid SessionId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AssetUrl { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? AssociatedTurnId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}
