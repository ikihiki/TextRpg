namespace CoreBackend.Features.CreateSession;

public class CreateSessionRequest
{
    public string ScenarioId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, object>? InitialState { get; set; }
}
