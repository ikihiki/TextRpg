namespace CoreBackend.Domains.Notes;

/// <summary>
/// 正史レベルを表す値オブジェクト（確定 / 仮説）
/// </summary>
public enum CanonLevel
{
    /// <summary>確定: 人間が確認した正史</summary>
    Confirmed,

    /// <summary>仮説: AIが提案した未確定の情報</summary>
    Hypothesis
}
