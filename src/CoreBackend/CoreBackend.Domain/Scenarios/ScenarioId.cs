namespace CoreBackend.Domain.Scenarios;

/// <summary>
/// シナリオIDの値オブジェクト
/// </summary>
public readonly record struct ScenarioId(Guid Value)
{
    public static ScenarioId New() => new(Guid.NewGuid());
    public static ScenarioId From(Guid value) => new(value);
    public static ScenarioId From(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();
}
