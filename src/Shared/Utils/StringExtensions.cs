namespace TextRpg.Shared.Utils;

/// <summary>
/// Common string utility extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is null, empty, or consists only of white-space characters.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
