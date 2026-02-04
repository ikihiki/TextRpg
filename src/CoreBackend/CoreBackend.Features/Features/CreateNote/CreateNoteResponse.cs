namespace CoreBackend.Features.CreateNote;

public class CreateNoteResponse
{
    public Guid NoteId { get; set; }
    public DateTime CreatedAt { get; set; }
}
