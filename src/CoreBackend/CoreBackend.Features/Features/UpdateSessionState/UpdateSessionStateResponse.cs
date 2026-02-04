namespace CoreBackend.Features.UpdateSessionState;

public class UpdateSessionStateResponse
{
    public bool Success { get; set; }
    public int NewVersion { get; set; }
    public DateTime UpdatedAt { get; set; }
}
