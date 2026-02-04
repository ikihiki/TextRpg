using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextRpg.Core;
using FeatureCreateNote = CoreBackend.Features.CreateNote;
using FeatureGetNotes = CoreBackend.Features.GetNotes;
using FeatureUpdateNote = CoreBackend.Features.UpdateNote;
using DomainNotes = CoreBackend.Domain.Notes;

namespace CoreBackend.Service.Services;

/// <summary>
/// gRPC service for managing Canonical Notes (Lorebook)
/// </summary>
public class NotesServiceImpl : NotesService.NotesServiceBase
{
    private readonly ILogger<NotesServiceImpl> _logger;
    private readonly FeatureCreateNote.CreateNote _createNote;
    private readonly FeatureGetNotes.GetNotes _getNotes;
    private readonly FeatureUpdateNote.UpdateNote _updateNote;

    public NotesServiceImpl(
        ILogger<NotesServiceImpl> logger,
        FeatureCreateNote.CreateNote createNote,
        FeatureGetNotes.GetNotes getNotes,
        FeatureUpdateNote.UpdateNote updateNote)
    {
        _logger = logger;
        _createNote = createNote;
        _getNotes = getNotes;
        _updateNote = updateNote;
    }

    public override Task<TextRpg.Core.CreateNoteResponse> CreateNote(
        TextRpg.Core.CreateNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating note {Name} for session {SessionId}",
            request.Name, request.SessionId);

        var result = _createNote.Execute(new FeatureCreateNote.CreateNoteRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            NoteType = MapNoteType(request.NoteType),
            Name = request.Name,
            Content = request.Content,
            CanonLevel = MapCanonLevel(request.CanonLevel),
            FirstTurnId = request.FirstTurnId > 0 ? request.FirstTurnId : null
        });

        return Task.FromResult(new TextRpg.Core.CreateNoteResponse
        {
            NoteId = result.NoteId.ToString(),
            CreatedAt = Timestamp.FromDateTime(result.CreatedAt)
        });
    }

    public override Task<GetNoteResponse> GetNote(GetNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting note {NoteId}", request.NoteId);

        // TODO: Implement get note logic
        return Task.FromResult(new GetNoteResponse());
    }

    public override Task<TextRpg.Core.UpdateNoteResponse> UpdateNote(
        TextRpg.Core.UpdateNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating note {NoteId}", request.NoteId);

        var result = _updateNote.Execute(new FeatureUpdateNote.UpdateNoteRequest
        {
            NoteId = Guid.Parse(request.NoteId),
            Name = request.Name,
            Content = request.Content,
            CanonLevel = request.CanonLevel != TextRpg.Core.CanonLevel.Unspecified
                ? MapCanonLevel(request.CanonLevel)
                : null
        });

        return Task.FromResult(new TextRpg.Core.UpdateNoteResponse
        {
            Success = result.Success,
            UpdatedAt = Timestamp.FromDateTime(result.UpdatedAt)
        });
    }

    public override Task<DeleteNoteResponse> DeleteNote(
        DeleteNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting note {NoteId}", request.NoteId);

        // TODO: Implement delete note logic
        return Task.FromResult(new DeleteNoteResponse { Success = true });
    }

    public override Task<ListNotesResponse> ListNotes(ListNotesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Listing notes for session {SessionId}", request.SessionId);

        var result = _getNotes.Execute(new FeatureGetNotes.GetNotesRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            TypeFilter = request.TypeFilter != TextRpg.Core.NoteType.Unspecified
                ? MapNoteType(request.TypeFilter)
                : null,
            MinCanonLevel = request.MinCanonLevel != TextRpg.Core.CanonLevel.Unspecified
                ? MapCanonLevel(request.MinCanonLevel)
                : null,
            PinnedOnly = request.PinnedOnly,
            Page = request.Page > 0 ? request.Page : 1,
            PageSize = request.PageSize > 0 ? request.PageSize : 20
        });

        var response = new ListNotesResponse
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        foreach (var note in result.Notes)
        {
            response.Notes.Add(MapToNoteData(note));
        }

        return Task.FromResult(response);
    }

    public override Task<SearchNotesResponse> SearchNotes(
        SearchNotesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Searching notes in session {SessionId} with query '{Query}'",
            request.SessionId, request.Query);

        // TODO: Implement search notes logic
        return Task.FromResult(new SearchNotesResponse());
    }

    public override Task<GetRelatedNotesResponse> GetRelatedNotes(
        GetRelatedNotesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting related notes for {NoteId}", request.NoteId);

        // TODO: Implement get related notes logic
        return Task.FromResult(new GetRelatedNotesResponse());
    }

    public override Task<GetNoteSuggestionResponse> GetNoteSuggestion(
        GetNoteSuggestionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting note suggestions for session {SessionId}", request.SessionId);

        // TODO: Implement note suggestion logic
        return Task.FromResult(new GetNoteSuggestionResponse());
    }

    public override Task<ApplyNoteSuggestionResponse> ApplyNoteSuggestion(
        ApplyNoteSuggestionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Applying note suggestion {SuggestionId}", request.SuggestionId);

        // TODO: Implement apply suggestion logic
        return Task.FromResult(new ApplyNoteSuggestionResponse { Success = true });
    }

    public override Task<CreateNoteThreadResponse> CreateNoteThread(
        CreateNoteThreadRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating note thread {ThreadName} in session {SessionId}",
            request.ThreadName, request.SessionId);

        // TODO: Implement create thread logic
        return Task.FromResult(new CreateNoteThreadResponse
        {
            ThreadId = Guid.NewGuid().ToString()
        });
    }

    public override Task<LinkNotesResponse> LinkNotes(LinkNotesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Linking notes {SourceNoteId} -> {TargetNoteId}",
            request.SourceNoteId, request.TargetNoteId);

        // TODO: Implement link notes logic
        return Task.FromResult(new LinkNotesResponse { Success = true });
    }

    public override Task<PinNoteResponse> PinNote(PinNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Setting pin status for note {NoteId} to {Pin}",
            request.NoteId, request.Pin);

        // TODO: Implement pin note logic
        return Task.FromResult(new PinNoteResponse { Success = true });
    }

    public override Task<AnchorNoteResponse> AnchorNote(AnchorNoteRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Setting anchor status for note {NoteId} to {Anchor}",
            request.NoteId, request.Anchor);

        // TODO: Implement anchor note logic
        return Task.FromResult(new AnchorNoteResponse { Success = true });
    }

    private static DomainNotes.NoteType MapNoteType(TextRpg.Core.NoteType noteType)
    {
        return noteType switch
        {
            TextRpg.Core.NoteType.Character => DomainNotes.NoteType.Pin,
            TextRpg.Core.NoteType.Location => DomainNotes.NoteType.Anchor,
            _ => DomainNotes.NoteType.Thread
        };
    }

    private static DomainNotes.CanonLevel MapCanonLevel(TextRpg.Core.CanonLevel canonLevel)
    {
        return canonLevel switch
        {
            TextRpg.Core.CanonLevel.Canon => DomainNotes.CanonLevel.Confirmed,
            TextRpg.Core.CanonLevel.Rumor => DomainNotes.CanonLevel.Hypothesis,
            TextRpg.Core.CanonLevel.Tentative => DomainNotes.CanonLevel.Hypothesis,
            _ => DomainNotes.CanonLevel.Hypothesis
        };
    }

    private static TextRpg.Core.NoteType MapToProtoNoteType(DomainNotes.NoteType noteType)
    {
        return noteType switch
        {
            DomainNotes.NoteType.Pin => TextRpg.Core.NoteType.Character,
            DomainNotes.NoteType.Anchor => TextRpg.Core.NoteType.Location,
            DomainNotes.NoteType.Thread => TextRpg.Core.NoteType.Event,
            _ => TextRpg.Core.NoteType.Unspecified
        };
    }

    private static TextRpg.Core.CanonLevel MapToProtoCanonLevel(DomainNotes.CanonLevel canonLevel)
    {
        return canonLevel switch
        {
            DomainNotes.CanonLevel.Confirmed => TextRpg.Core.CanonLevel.Canon,
            DomainNotes.CanonLevel.Hypothesis => TextRpg.Core.CanonLevel.Tentative,
            _ => TextRpg.Core.CanonLevel.Unspecified
        };
    }

    private static TextRpg.Core.NoteData MapToNoteData(DomainNotes.NoteData note)
    {
        var noteData = new TextRpg.Core.NoteData
        {
            NoteId = note.NoteId.ToString(),
            SessionId = note.SessionId.ToString(),
            NoteType = MapToProtoNoteType(note.NoteType),
            Name = note.Name,
            Content = note.Content,
            CanonLevel = MapToProtoCanonLevel(note.CanonLevel),
            IsPinned = note.IsPinned,
            IsAnchored = note.IsAnchored,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(note.CreatedAt, DateTimeKind.Utc)),
            UpdatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(note.UpdatedAt, DateTimeKind.Utc))
        };

        if (note.FirstTurnId.HasValue)
            noteData.FirstTurnId = note.FirstTurnId.Value;
        if (note.LastUpdatedTurnId.HasValue)
            noteData.LastUpdatedTurnId = note.LastUpdatedTurnId.Value;

        noteData.Aliases.AddRange(note.Aliases);
        noteData.TagIds.AddRange(note.TagIds);
        noteData.ThreadIds.AddRange(note.ThreadIds);
        noteData.EvidenceTurnIds.AddRange(note.EvidenceTurnIds);

        return noteData;
    }
}
