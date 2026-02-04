namespace CoreBackend.Features.ValidateSession;

public class ValidateSessionRequest
{
    public required string SessionToken { get; init; }
}
