namespace AIOrchestrator.Domain.Narrative;

/// <summary>
/// 物語生成結果
/// </summary>
public class NarrativeResult
{
    public string Text { get; set; } = string.Empty;
    public List<string> SuggestedActions { get; set; } = new();
    public List<NoteSuggestion> NoteSuggestions { get; set; } = new();

    // TODO: Implement narrative result logic
}

public class NoteSuggestion
{
    public string Content { get; set; } = string.Empty;
    public string SuggestedType { get; set; } = string.Empty;
}
