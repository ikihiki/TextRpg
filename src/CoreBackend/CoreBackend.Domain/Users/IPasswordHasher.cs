namespace CoreBackend.Domain.Users;

/// <summary>
/// パスワードハッシュサービスのインターフェース
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
