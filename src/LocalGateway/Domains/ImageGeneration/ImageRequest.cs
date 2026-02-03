namespace LocalGateway.Domains.ImageGeneration;

/// <summary>
/// 画像生成リクエスト
/// </summary>
public class ImageRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string NegativePrompt { get; set; } = string.Empty;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public int Steps { get; set; } = 30;

    // TODO: Implement image request logic
}
