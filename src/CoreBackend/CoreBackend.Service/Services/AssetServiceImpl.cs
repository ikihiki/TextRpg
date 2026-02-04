using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TextRpg.Core;
using FeatureCreateAsset = CoreBackend.Features.CreateAsset;
using FeatureGetAssets = CoreBackend.Features.GetAssets;
using DomainAssets = CoreBackend.Domain.Assets;

namespace CoreBackend.Service.Services;

/// <summary>
/// gRPC service for managing assets (illustrations, audio, etc.)
/// </summary>
public class AssetServiceImpl : AssetService.AssetServiceBase
{
    private readonly ILogger<AssetServiceImpl> _logger;
    private readonly FeatureCreateAsset.CreateAsset _createAsset;
    private readonly FeatureGetAssets.GetAssets _getAssets;

    public AssetServiceImpl(
        ILogger<AssetServiceImpl> logger,
        FeatureCreateAsset.CreateAsset createAsset,
        FeatureGetAssets.GetAssets getAssets)
    {
        _logger = logger;
        _createAsset = createAsset;
        _getAssets = getAssets;
    }

    public override Task<TextRpg.Core.CreateAssetResponse> CreateAsset(
        TextRpg.Core.CreateAssetRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating asset {Name} for session {SessionId}",
            request.Name, request.SessionId);

        var result = _createAsset.Execute(new FeatureCreateAsset.CreateAssetRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            AssetType = request.AssetType.ToString(),
            Name = request.Name,
            Description = request.Description,
            AssetData = request.AssetData?.ToByteArray(),
            MimeType = request.MimeType,
            AssociatedTurnId = request.AssociatedTurnId > 0 ? request.AssociatedTurnId : null
        });

        return Task.FromResult(new TextRpg.Core.CreateAssetResponse
        {
            AssetId = result.AssetId.ToString(),
            AssetUrl = result.AssetUrl,
            CreatedAt = Timestamp.FromDateTime(result.CreatedAt)
        });
    }

    public override Task<GetAssetResponse> GetAsset(GetAssetRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting asset {AssetId}", request.AssetId);

        // TODO: Implement get asset logic
        return Task.FromResult(new GetAssetResponse());
    }

    public override Task<ListAssetsResponse> ListAssets(ListAssetsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Listing assets for session {SessionId}", request.SessionId);

        var result = _getAssets.Execute(new FeatureGetAssets.GetAssetsRequest
        {
            SessionId = Guid.Parse(request.SessionId),
            TypeFilter = request.TypeFilter != AssetType.Unspecified ? request.TypeFilter.ToString() : null,
            Page = request.Page > 0 ? request.Page : 1,
            PageSize = request.PageSize > 0 ? request.PageSize : 20
        });

        var response = new ListAssetsResponse
        {
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        foreach (var asset in result.Assets)
        {
            response.Assets.Add(MapToAssetData(asset));
        }

        return Task.FromResult(response);
    }

    public override Task<DeleteAssetResponse> DeleteAsset(
        DeleteAssetRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting asset {AssetId}", request.AssetId);

        // TODO: Implement delete asset logic
        return Task.FromResult(new DeleteAssetResponse { Success = true });
    }

    public override Task<RequestIllustrationResponse> RequestIllustration(
        RequestIllustrationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Requesting illustration for session {SessionId}, turn {TurnId}",
            request.SessionId, request.TurnId);

        // TODO: Implement illustration request logic (queue to Hangfire job)
        return Task.FromResult(new RequestIllustrationResponse
        {
            JobId = Guid.NewGuid().ToString(),
            Status = IllustrationStatus.Queued
        });
    }

    public override Task<GetIllustrationStatusResponse> GetIllustrationStatus(
        GetIllustrationStatusRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting illustration status for job {JobId}", request.JobId);

        // TODO: Implement get illustration status logic
        return Task.FromResult(new GetIllustrationStatusResponse
        {
            Status = IllustrationStatus.Processing,
            ProgressPercentage = 0
        });
    }

    public override Task<UpdateVisualCanonResponse> UpdateVisualCanon(
        UpdateVisualCanonRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating visual canon for session {SessionId}", request.SessionId);

        // TODO: Implement update visual canon logic
        return Task.FromResult(new UpdateVisualCanonResponse { Success = true });
    }

    public override Task<GetVisualCanonResponse> GetVisualCanon(
        GetVisualCanonRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting visual canon for session {SessionId}", request.SessionId);

        // TODO: Implement get visual canon logic
        return Task.FromResult(new GetVisualCanonResponse());
    }

    private static TextRpg.Core.AssetData MapToAssetData(DomainAssets.AssetData asset)
    {
        var assetData = new TextRpg.Core.AssetData
        {
            AssetId = asset.AssetId.ToString(),
            SessionId = asset.SessionId.ToString(),
            AssetType = System.Enum.TryParse<AssetType>(asset.AssetType, true, out var at) ? at : AssetType.Unspecified,
            Name = asset.Name,
            Description = asset.Description,
            AssetUrl = asset.AssetUrl,
            MimeType = asset.MimeType,
            FileSize = asset.FileSize,
            CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(asset.CreatedAt, DateTimeKind.Utc))
        };

        if (asset.AssociatedTurnId.HasValue)
            assetData.AssociatedTurnId = asset.AssociatedTurnId.Value;

        return assetData;
    }
}
