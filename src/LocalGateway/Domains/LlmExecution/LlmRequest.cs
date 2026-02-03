namespace LocalGateway.Domains.LlmExecution;

/// <summary>
/// LLMリクエスト
/// </summary>
public class LlmRequest
{
    public string Prompt { get; set; } = string.Empty;
    public int MaxTokens { get; set; }
    public float Temperature { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new();

    // TODO: Implement LLM request logic
}
