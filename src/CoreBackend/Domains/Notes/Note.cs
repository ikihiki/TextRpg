namespace CoreBackend.Domains.Notes;

/// <summary>
/// ノートの集約ルートエンティティ（正史ノート）
/// PIN / ANCHOR / THREADSによる記憶管理
/// </summary>
public class Note
{
    public Guid Id { get; private set; }
    public Guid SessionId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public NoteType Type { get; private set; }
    public CanonLevel CanonLevel { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Extensible note data
    public Dictionary<string, object> Ext { get; set; } = new();

    // TODO: Implement note logic with evidence (根拠ログ) support
}
