using CoreBackend.Domain.Notes;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.GetNotes;

public class GetNotes : IUseCase<GetNotesRequest, GetNotesResponse>
{
    public GetNotesResponse Execute(GetNotesRequest request)
    {
        // TODO: Retrieve notes from database via repository with filtering
        return new GetNotesResponse
        {
            Notes = new List<NoteData>(),
            TotalCount = 0,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
