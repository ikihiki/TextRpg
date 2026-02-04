namespace CoreBackend.Domain.Assets;

/// <summary>
/// アセットエンティティ（挿絵など）
/// </summary>
public class Asset
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string AssetType { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    // TODO: Implement asset logic
}
