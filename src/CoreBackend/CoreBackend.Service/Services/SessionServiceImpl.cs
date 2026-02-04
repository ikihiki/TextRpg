using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextRpg.Core;
using FeatureCreateSession = CoreBackend.Features.CreateSession;
using FeatureGetSession = CoreBackend.Features.GetSession;
using FeatureUpdateSessionState = CoreBackend.Features.UpdateSessionState;
using FeatureCreateTurn = CoreBackend.Features.CreateTurn;
using FeatureRewindTurn = CoreBackend.Features.RewindTurn;

namespace CoreBackend.Service.Services;

/// <summary>
/// gRPC service for session and state management
/// </summary>
public class SessionServiceImpl : SessionService.SessionServiceBase
{
    private readonly ILogger<SessionServiceImpl> _logger;
    private readonly FeatureCreateSession.CreateSession _createSession;
    private readonly FeatureGetSession.GetSession _getSession;
    private readonly FeatureUpdateSessionState.UpdateSessionState _updateSessionState;
    private readonly FeatureCreateTurn.CreateTurn _createTurn;
    private readonly FeatureRewindTurn.RewindTurn _rewindTurn;

    public SessionServiceImpl(
        ILogger<SessionServiceImpl> logger,
        FeatureCreateSession.CreateSession createSession,
        FeatureGetSession.GetSession getSession,
        FeatureUpdateSessionState.UpdateSessionState updateSessionState,
        FeatureCreateTurn.CreateTurn createTurn,
        FeatureRewindTurn.RewindTurn rewindTurn)
    {
        _logger = logger;
        _createSession = createSession;
        _getSession = getSession;
        _updateSessionState = updateSessionState;
        _createTurn = createTurn;
        _rewindTurn = rewindTurn;
    }

    public override Task<TextRpg.Core.CreateSessionResponse> CreateSession(
        TextRpg.Core.CreateSessionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating session for scenario {ScenarioId}, user {UserId}",
            request.ScenarioId, request.UserId);

        var result = _createSession.Execute(new FeatureCreateSession.CreateSessionRequest
        {
            ScenarioId = request.ScenarioId,
            UserId = request.UserId
        });

        return Task.FromResult(new TextRpg.Core.CreateSessionResponse
        {
            SessionId = result.SessionId.ToString(),
            CreatedAt = Timestamp.FromDateTime(result.CreatedAt)
        });
    }

    public override Task<TextRpg.Core.GetSessionResponse> GetSession(
        TextRpg.Core.GetSessionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting session {SessionId}", request.SessionId);

        var result = _getSession.Execute(new FeatureGetSession.GetSessionRequest
        {
            SessionId = Guid.Parse(request.SessionId)
        });

        if (result == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Session {request.SessionId} not found"));
        }

        return Task.FromResult(new TextRpg.Core.GetSessionResponse
        {
            Session = new SessionData
            {
                SessionId = result.SessionId.ToString(),
                ScenarioId = result.ScenarioId,
                UserId = result.UserId,
                CurrentTurn = result.CurrentTurn,
                CurrentChapter = result.CurrentChapter,
                CreatedAt = Timestamp.FromDateTime(result.CreatedAt),
                UpdatedAt = Timestamp.FromDateTime(result.UpdatedAt)
            }
        });
    }

    public override Task<UpdateSessionStatusResponse> UpdateSessionStatus(
        UpdateSessionStatusRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating session status for {SessionId} to {Status}",
            request.SessionId, request.Status);

        // TODO: Implement status update logic
        return Task.FromResult(new UpdateSessionStatusResponse
        {
            Success = true,
            UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }

    public override Task<ListUserSessionsResponse> ListUserSessions(
        ListUserSessionsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Listing sessions for user {UserId}", request.UserId);

        // TODO: Implement listing logic
        return Task.FromResult(new ListUserSessionsResponse
        {
            TotalCount = 0,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }

    public override Task<GetSessionStateResponse> GetSessionState(
        GetSessionStateRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting session state for {SessionId}", request.SessionId);

        // TODO: Implement get state logic
        return Task.FromResult(new GetSessionStateResponse
        {
            State = new SessionState
            {
                SessionId = request.SessionId,
                Version = 1,
                UpdatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
            }
        });
    }

    public override Task<TextRpg.Core.UpdateSessionStateResponse> UpdateSessionState(
        TextRpg.Core.UpdateSessionStateRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating session state for {SessionId}", request.SessionId);

        var result = _updateSessionState.Execute(new FeatureUpdateSessionState.UpdateSessionStateRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            ExpectedVersion = request.ExpectedVersion
        });

        return Task.FromResult(new TextRpg.Core.UpdateSessionStateResponse
        {
            Success = result.Success,
            NewVersion = result.NewVersion,
            UpdatedAt = Timestamp.FromDateTime(result.UpdatedAt)
        });
    }

    public override Task<TextRpg.Core.CreateTurnResponse> CreateTurn(
        TextRpg.Core.CreateTurnRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating turn {TurnNumber} for session {SessionId}",
            request.TurnNumber, request.SessionId);

        var result = _createTurn.Execute(new FeatureCreateTurn.CreateTurnRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            TurnNumber = request.TurnNumber,
            PlayerInput = request.PlayerInput,
            Narrative = request.Narrative,
            TurnType = request.TurnType.ToString()
        });

        return Task.FromResult(new TextRpg.Core.CreateTurnResponse
        {
            TurnId = result.TurnId,
            CreatedAt = Timestamp.FromDateTime(result.CreatedAt)
        });
    }

    public override Task<GetTurnResponse> GetTurn(GetTurnRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting turn {TurnId} for session {SessionId}",
            request.TurnId, request.SessionId);

        // TODO: Implement get turn logic
        return Task.FromResult(new GetTurnResponse());
    }

    public override Task<GetTurnsResponse> GetTurns(GetTurnsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting turns for session {SessionId}", request.SessionId);

        // TODO: Implement get turns logic
        return Task.FromResult(new GetTurnsResponse());
    }

    public override Task<InvalidateTurnsAfterResponse> InvalidateTurnsAfter(
        InvalidateTurnsAfterRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Invalidating turns after {TurnId} for session {SessionId}",
            request.TargetTurnId, request.SessionId);

        var result = _rewindTurn.Execute(new FeatureRewindTurn.RewindTurnRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            TargetTurnId = request.TargetTurnId
        });

        return Task.FromResult(new InvalidateTurnsAfterResponse
        {
            Success = result.Success,
            InvalidatedCount = result.InvalidatedCount
        });
    }

    public override Task<CreateChapterResponse> CreateChapter(
        CreateChapterRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating chapter for session {SessionId}", request.SessionId);

        // TODO: Implement create chapter logic
        return Task.FromResult(new CreateChapterResponse
        {
            ChapterId = 1,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }

    public override Task<GetChaptersResponse> GetChapters(
        GetChaptersRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting chapters for session {SessionId}", request.SessionId);

        // TODO: Implement get chapters logic
        return Task.FromResult(new GetChaptersResponse());
    }

    public override Task<UpdateChapterSummaryResponse> UpdateChapterSummary(
        UpdateChapterSummaryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating chapter summary for chapter {ChapterId}", request.ChapterId);

        // TODO: Implement update chapter summary logic
        return Task.FromResult(new UpdateChapterSummaryResponse { Success = true });
    }

    public override Task<BuildAIContextResponse> BuildAIContext(
        BuildAIContextRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Building AI context for session {SessionId}", request.SessionId);

        // TODO: Implement AI context building logic
        return Task.FromResult(new BuildAIContextResponse
        {
            Context = new AIContext
            {
                SessionId = request.SessionId
            }
        });
    }

    public override Task<SetProtagonistResponse> SetProtagonist(
        SetProtagonistRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Setting protagonist for session {SessionId}", request.SessionId);

        // TODO: Implement set protagonist logic
        return Task.FromResult(new SetProtagonistResponse { Success = true });
    }

    public override Task<GetProtagonistResponse> GetProtagonist(
        GetProtagonistRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting protagonist for session {SessionId}", request.SessionId);

        // TODO: Implement get protagonist logic
        return Task.FromResult(new GetProtagonistResponse());
    }
}
