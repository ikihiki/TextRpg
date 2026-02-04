using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.UpdateSessionState;

public class UpdateSessionState : IUseCase<UpdateSessionStateRequest, UpdateSessionStateResponse>
{
    public UpdateSessionStateResponse Execute(UpdateSessionStateRequest request)
    {
        // TODO: Update session state in database via repository
        // Implement optimistic locking with version check
        return new UpdateSessionStateResponse
        {
            Success = true,
            NewVersion = request.ExpectedVersion + 1,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
