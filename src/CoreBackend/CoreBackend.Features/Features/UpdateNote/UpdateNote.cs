using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.UpdateNote;

public class UpdateNote : IUseCase<UpdateNoteRequest, UpdateNoteResponse>
{
    public UpdateNoteResponse Execute(UpdateNoteRequest request)
    {
        // TODO: Update note in database via repository
        return new UpdateNoteResponse
        {
            Success = true,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
