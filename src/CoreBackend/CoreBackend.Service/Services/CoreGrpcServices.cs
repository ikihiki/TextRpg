using TextRpg.Core;

namespace CoreBackend.Service.Services;

// The generated base classes already return UNIMPLEMENTED for RPCs we have not built yet.
public sealed class SessionGrpcService : SessionService.SessionServiceBase;

public sealed class NotesGrpcService : NotesService.NotesServiceBase;

public sealed class AssetGrpcService : AssetService.AssetServiceBase;
