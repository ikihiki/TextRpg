namespace CoreBackend.Domain.Assets;

/// <summary>
/// Visual Canon（見た目の正史）
/// 挿絵生成時の一貫性を保つための参照データ
/// </summary>
public class VisualCanon
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string CharacterId { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ReferenceImageUrl { get; private set; }

    // TODO: Implement visual canon logic
}
