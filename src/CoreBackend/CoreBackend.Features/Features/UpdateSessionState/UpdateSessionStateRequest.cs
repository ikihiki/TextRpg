namespace CoreBackend.Features.UpdateSessionState;

public class UpdateSessionStateRequest
{
    public Guid SessionId { get; set; }
    public int ExpectedVersion { get; set; }
    public Dictionary<string, object>? StateData { get; set; }
    public Dictionary<string, object>? Extensions { get; set; }
}
