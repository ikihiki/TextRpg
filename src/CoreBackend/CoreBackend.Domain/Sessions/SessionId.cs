namespace CoreBackend.Domain.Sessions;

/// <summary>
/// セッションIDの値オブジェクト
/// </summary>
public readonly record struct SessionId(Guid Value)
{
    public static SessionId New() => new(Guid.NewGuid());
    public static SessionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
