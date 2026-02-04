namespace CoreBackend.Features.CreateTurn;

public class CreateTurnRequest
{
    public Guid SessionId { get; set; }
    public int TurnNumber { get; set; }
    public string PlayerInput { get; set; } = string.Empty;
    public string Narrative { get; set; } = string.Empty;
    public string TurnType { get; set; } = string.Empty;
    public Dictionary<string, object>? Metadata { get; set; }
}
