using CoreBackend.Domain.Notes;

namespace CoreBackend.Features.CreateNote;

public class CreateNoteRequest
{
    public Guid SessionId { get; set; }
    public NoteType NoteType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public CanonLevel CanonLevel { get; set; }
    public int? FirstTurnId { get; set; }
    public Dictionary<string, object>? StructuredData { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
}
