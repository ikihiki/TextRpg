namespace AIOrchestrator.Domains.Narrative;

/// <summary>
/// 物語生成器のインターフェース
/// AIは物語描写、行動候補生成、ノート差分提案、戦闘ログの文章化を担当
/// </summary>
public interface INarrativeGenerator
{
    Task<NarrativeResult> GenerateIntroAsync(NarrativeContext context, CancellationToken cancellationToken = default);
    Task<NarrativeResult> GenerateGameplayAsync(NarrativeContext context, string playerAction, CancellationToken cancellationToken = default);
}
