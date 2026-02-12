using CoreBackend.Domain.Users;
using VerticalSliceArchitecture.Core;

namespace CoreBackend.Features.RegisterUser;

public class RegisterUser : IUseCase<RegisterUserRequest, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUser(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        // Validate email format
        if (!IsValidEmail(request.Email))
        {
            return new RegisterUserResponse
            {
                Success = false,
                ErrorMessage = "Invalid email format"
            };
        }

        // Validate password requirements
        if (!IsValidPassword(request.Password))
        {
            return new RegisterUserResponse
            {
                Success = false,
                ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, and digit"
            };
        }

        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return new RegisterUserResponse
            {
                Success = false,
                ErrorMessage = "Email already registered"
            };
        }

        // Create user
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResponse
        {
            Success = true,
            UserId = user.Id.ToString()
        };
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);

        return hasUpper && hasLower && hasDigit;
    }
}
