namespace CoreBackend.Features.CreateAsset;

public class CreateAssetResponse
{
    public Guid AssetId { get; set; }
    public string AssetUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
