using CoreBackend.Domain.Notes;

namespace CoreBackend.Features.GetNotes;

public class GetNotesRequest
{
    public Guid SessionId { get; set; }
    public NoteType? TypeFilter { get; set; }
    public CanonLevel? MinCanonLevel { get; set; }
    public bool? PinnedOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderBy { get; set; }
}
