namespace CoreBackend.Domain.Scenarios;

/// <summary>
/// シナリオの集約ルートエンティティ
/// </summary>
public class Scenario
{
    // EF Core用のプライベートコンストラクタ
    private Scenario() { }

    private Scenario(ScenarioId id, string title, string? summary, string ownerId)
    {
        Id = id;
        Title = title;
        Summary = summary;
        OwnerId = ownerId;
        Status = ScenarioStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ScenarioId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Summary { get; private set; }
    public string OwnerId { get; private set; } = string.Empty;
    public ScenarioStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// 新しいシナリオを作成する
    /// </summary>
    /// <param name="title">シナリオタイトル（必須）</param>
    /// <param name="summary">概要（任意）</param>
    /// <param name="ownerId">所有者のユーザーID</param>
    /// <returns>下書き状態の新しいシナリオ</returns>
    public static Scenario Create(string title, string? summary, string ownerId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("タイトルは必須です", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(ownerId))
        {
            throw new ArgumentException("所有者IDは必須です", nameof(ownerId));
        }

        return new Scenario(ScenarioId.New(), title, summary, ownerId);
    }

    /// <summary>
    /// シナリオのタイトルを更新する
    /// </summary>
    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("タイトルは必須です", nameof(title));
        }

        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// シナリオの概要を更新する
    /// </summary>
    public void UpdateSummary(string? summary)
    {
        Summary = summary;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// シナリオを公開する
    /// </summary>
    public void Publish()
    {
        Status = ScenarioStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// シナリオをアーカイブする
    /// </summary>
    public void Archive()
    {
        Status = ScenarioStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}
