using CoreBackend.Domain.Notes;

namespace CoreBackend.Features.GetNotes;

public class GetNotesResponse
{
    public List<NoteData> Notes { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
