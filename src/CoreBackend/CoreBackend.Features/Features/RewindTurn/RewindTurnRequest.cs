namespace CoreBackend.Features.RewindTurn;

public class RewindTurnRequest
{
    public Guid SessionId { get; set; }
    public int TargetTurnId { get; set; }
}
