namespace CoreBackend.Features.CreateAsset;

public class CreateAssetRequest
{
    public Guid SessionId { get; set; }
    public string AssetType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte[]? AssetData { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public int? AssociatedTurnId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
