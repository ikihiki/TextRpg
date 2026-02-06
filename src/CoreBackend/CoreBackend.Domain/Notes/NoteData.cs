namespace CoreBackend.Domain.Notes;

/// <summary>
/// Data transfer object for Note information
/// </summary>
public class NoteData
{
    public Guid NoteId { get; set; }
    public Guid SessionId { get; set; }
    public NoteType NoteType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> Aliases { get; set; } = new();
    public CanonLevel CanonLevel { get; set; }
    public bool IsPinned { get; set; }
    public bool IsAnchored { get; set; }
    public int? FirstTurnId { get; set; }
    public int? LastUpdatedTurnId { get; set; }
    public Dictionary<string, object>? StructuredData { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
    public List<string> TagIds { get; set; } = new();
    public List<string> ThreadIds { get; set; } = new();
    public List<int> EvidenceTurnIds { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
