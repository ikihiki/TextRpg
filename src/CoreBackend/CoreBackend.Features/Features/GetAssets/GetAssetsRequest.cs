namespace CoreBackend.Features.GetAssets;

public class GetAssetsRequest
{
    public Guid SessionId { get; set; }
    public string? TypeFilter { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? OrderBy { get; set; }
}
