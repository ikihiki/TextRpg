using System;
using System.IO;
using System.Linq;

namespace VerticalSliceArchitecture.Analyzers;

/// <summary>
/// Helper class to extract feature information from file paths.
/// </summary>
internal static class FeaturePathHelper
{
    /// <summary>
    /// Determines if a file path is within a Features project.
    /// </summary>
    public static bool IsFeaturesProject(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        var normalized = filePath.Replace('\\', '/');
        return normalized.Contains(".Features/") || normalized.Contains(".Features\\");
    }

    /// <summary>
    /// Extracts the feature name from a file path.
    /// Expected pattern: <ProjectRoot>/Features/<FeatureName>/.../*.cs
    /// </summary>
    /// <returns>The feature name if found, otherwise null.</returns>
    public static string? ExtractFeatureName(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return null;

        var normalized = filePath.Replace('\\', '/');
        var parts = normalized.Split('/');

        // Find "Features" folder index
        var featuresIndex = Array.FindIndex(parts, p => p.Equals("Features", StringComparison.OrdinalIgnoreCase));
        
        if (featuresIndex == -1 || featuresIndex >= parts.Length - 1)
            return null;

        // Feature name is the folder immediately after "Features"
        return parts[featuresIndex + 1];
    }

    /// <summary>
    /// Extracts the unit name from a project name or file path.
    /// Expected pattern: <Unit>.Features
    /// </summary>
    public static string? ExtractUnitName(string projectNameOrPath)
    {
        if (string.IsNullOrEmpty(projectNameOrPath))
            return null;

        // Get just the filename without path
        var fileName = Path.GetFileName(projectNameOrPath);
        
        var suffix = ".Features";
        if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return fileName.Substring(0, fileName.Length - suffix.Length);
        }

        return null;
    }

    /// <summary>
    /// Builds the expected namespace for a feature.
    /// Pattern: <Unit>.Features.<FeatureName>
    /// </summary>
    public static string BuildExpectedNamespace(string unitName, string featureName)
    {
        if (string.IsNullOrEmpty(unitName) || string.IsNullOrEmpty(featureName))
            throw new ArgumentException("Unit name and feature name cannot be null or empty");

        return $"{unitName}.Features.{featureName}";
    }

    /// <summary>
    /// Checks if a namespace matches the expected Features project pattern.
    /// Pattern: <Unit>.Features.<FeatureName>
    /// </summary>
    public static bool IsValidFeaturesNamespace(string namespaceName, out string? unitName, out string? featureName)
    {
        unitName = null;
        featureName = null;

        if (string.IsNullOrEmpty(namespaceName))
            return false;

        var parts = namespaceName.Split('.');
        
        // Must have at least 3 parts: <Unit>.Features.<FeatureName>
        if (parts.Length < 3)
            return false;

        // Second part must be "Features"
        if (!parts[parts.Length - 2].Equals("Features", StringComparison.Ordinal))
            return false;

        featureName = parts[parts.Length - 1];
        unitName = string.Join(".", parts.Take(parts.Length - 2));

        return true;
    }
}
