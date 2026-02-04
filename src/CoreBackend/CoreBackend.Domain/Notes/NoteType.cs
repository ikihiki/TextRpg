namespace CoreBackend.Domain.Notes;

/// <summary>
/// ノートの種類を表す値オブジェクト
/// </summary>
public enum NoteType
{
    /// <summary>PIN: 固定された重要な情報</summary>
    Pin,

    /// <summary>ANCHOR: 物語の基準点</summary>
    Anchor,

    /// <summary>THREADS: 物語の糸（進行中の要素）</summary>
    Thread
}
