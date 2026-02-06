using CoreBackend.Domain.Notes;

namespace CoreBackend.Features.UpdateNote;

public class UpdateNoteRequest
{
    public Guid NoteId { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<string>? Aliases { get; set; }
    public CanonLevel? CanonLevel { get; set; }
    public Dictionary<string, object>? StructuredData { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
    public int? LastUpdatedTurnId { get; set; }
}
