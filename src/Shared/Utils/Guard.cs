namespace TextRpg.Shared.Utils;

/// <summary>
/// Common guard clause utilities for input validation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws ArgumentNullException if the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's not null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public static T NotNull<T>(T? value, string parameterName) where T : class
    {
        return value ?? throw new ArgumentNullException(parameterName);
    }

    /// <summary>
    /// Throws ArgumentException if the string value is null or empty.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null or empty.</exception>
    public static string NotNullOrEmpty(string? value, string parameterName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
        }
        return value;
    }

    /// <summary>
    /// Throws ArgumentException if the string value is null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's not null, empty, or whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown when value is null, empty, or whitespace.</exception>
    public static string NotNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", parameterName);
        }
        return value;
    }
}
