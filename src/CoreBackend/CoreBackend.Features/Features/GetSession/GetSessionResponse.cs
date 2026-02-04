namespace CoreBackend.Features.GetSession;

public class GetSessionResponse
{
    public Guid SessionId { get; set; }
    public string ScenarioId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CurrentTurn { get; set; }
    public int CurrentChapter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastPlayedAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
