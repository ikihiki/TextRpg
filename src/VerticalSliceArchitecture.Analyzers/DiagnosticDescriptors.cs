using Microsoft.CodeAnalysis;

namespace VerticalSliceArchitecture.Analyzers;

/// <summary>
/// Diagnostic descriptors for the Vertical Slice Architecture analyzer.
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "Architecture";

    public static readonly DiagnosticDescriptor VSA001_FeatureFolderNamespaceMismatch = new(
        id: "VSA001",
        title: "Feature folder name and namespace mismatch",
        messageFormat: "Namespace '{0}' does not match expected '{1}' based on file path.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The namespace must match the feature folder structure: <Unit>.Features.<FeatureName>.");

    public static readonly DiagnosticDescriptor VSA002_InvalidNamespace = new(
        id: "VSA002",
        title: "Invalid namespace format",
        messageFormat: "Namespace '{0}' is not in the correct format for a Features project. Expected format: <Unit>.Features.<FeatureName>.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Namespaces in *.Features projects must follow the pattern: <Unit>.Features.<FeatureName>.");

    public static readonly DiagnosticDescriptor VSA010_UnauthorizedPublicType = new(
        id: "VSA010",
        title: "Unauthorized public type",
        messageFormat: "Type '{0}' cannot be public. Only UseCases and their Request/Response types are allowed to be public.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only IUseCase implementations and their Request/Response types can be public in Features projects.");

    public static readonly DiagnosticDescriptor VSA011_IndirectUseCaseImplementation = new(
        id: "VSA011",
        title: "Indirect IUseCase implementation",
        messageFormat: "UseCase '{0}' must implement IUseCase<,> directly, not through inheritance.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "UseCases must implement IUseCase<TRequest, TResponse> directly in their class declaration.");

    public static readonly DiagnosticDescriptor VSA020_RequestResponseSharingViolation = new(
        id: "VSA020",
        title: "Request/Response sharing violation",
        messageFormat: "Type '{0}' is shared by multiple UseCases within the same Features assembly. Each UseCase must have unique Request/Response types from the same assembly.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Request/Response types defined in the same Features assembly cannot be shared between multiple UseCases.");

    public static readonly DiagnosticDescriptor VSA030_FeatureCrossReferenceViolation = new(
        id: "VSA030",
        title: "Feature cross-reference violation",
        messageFormat: "Feature '{0}' is referencing non-public type '{1}' from feature '{2}'. Only UseCase and Request/Response types can be referenced across features.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Features can only reference public UseCases and their Request/Response types from other features.");
}
