using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.CreateNote;

public class CreateNote : IUseCase<CreateNoteRequest, CreateNoteResponse>
{
    public CreateNoteResponse Execute(CreateNoteRequest request)
    {
        var noteId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // TODO: Persist note to database via repository
        return new CreateNoteResponse
        {
            NoteId = noteId,
            CreatedAt = createdAt
        };
    }
}
