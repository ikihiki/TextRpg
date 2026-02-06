using CoreBackend.Domain.Assets;

namespace CoreBackend.Features.GetAssets;

public class GetAssetsResponse
{
    public List<AssetData> Assets { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
