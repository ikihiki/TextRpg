using CoreBackend.Domain.Assets;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.GetAssets;

public class GetAssets : IUseCase<GetAssetsRequest, GetAssetsResponse>
{
    public GetAssetsResponse Execute(GetAssetsRequest request)
    {
        // TODO: Retrieve assets from database via repository with filtering
        return new GetAssetsResponse
        {
            Assets = new List<AssetData>(),
            TotalCount = 0,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
