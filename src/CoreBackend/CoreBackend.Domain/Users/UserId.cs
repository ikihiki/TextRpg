namespace CoreBackend.Domain.Users;

/// <summary>
/// ユーザーIDの値オブジェクト
/// </summary>
public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    public static UserId From(string value) => new(Guid.Parse(value));
    public override string ToString() => Value.ToString();
}
