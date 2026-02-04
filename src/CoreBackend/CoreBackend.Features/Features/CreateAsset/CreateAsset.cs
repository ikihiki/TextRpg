using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.CreateAsset;

public class CreateAsset : IUseCase<CreateAssetRequest, CreateAssetResponse>
{
    public CreateAssetResponse Execute(CreateAssetRequest request)
    {
        var assetId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // TODO: Store asset data and persist to database via repository
        return new CreateAssetResponse
        {
            AssetId = assetId,
            AssetUrl = $"/assets/{assetId}",
            CreatedAt = createdAt
        };
    }
}
