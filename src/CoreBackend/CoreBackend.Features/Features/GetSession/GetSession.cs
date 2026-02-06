using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.GetSession;

public class GetSession : IUseCase<GetSessionRequest, GetSessionResponse?>
{
    public GetSessionResponse? Execute(GetSessionRequest request)
    {
        // TODO: Retrieve session from database via repository
        // Return null if not found
        return null;
    }
}
