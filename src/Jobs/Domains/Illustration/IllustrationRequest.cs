namespace Jobs.Domains.Illustration;

/// <summary>
/// 挿絵リクエスト
/// </summary>
public class IllustrationRequest
{
    public Guid SessionId { get; set; }
    public int TurnNumber { get; set; }
    public string SceneDescription { get; set; } = string.Empty;
    public List<string> CharacterIds { get; set; } = new();
    public string Style { get; set; } = string.Empty;

    // TODO: Implement illustration request logic
}
