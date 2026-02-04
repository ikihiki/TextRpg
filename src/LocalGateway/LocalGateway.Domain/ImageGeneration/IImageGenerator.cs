namespace LocalGateway.Domain.ImageGeneration;

/// <summary>
/// 画像生成器のインターフェース
/// 自宅PCでの画像生成
/// </summary>
public interface IImageGenerator
{
    Task<byte[]> GenerateAsync(ImageRequest request, CancellationToken cancellationToken = default);
}
