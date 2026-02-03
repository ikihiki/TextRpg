using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VerticalSliceArchitecture.Analyzers;

/// <summary>
/// Analyzer that enforces vertical slice architecture rules on Features projects.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FeaturesArchitectureAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.VSA001_FeatureFolderNamespaceMismatch,
        DiagnosticDescriptors.VSA002_InvalidNamespace,
        DiagnosticDescriptors.VSA010_UnauthorizedPublicType,
        DiagnosticDescriptors.VSA011_IndirectUseCaseImplementation,
        DiagnosticDescriptors.VSA020_RequestResponseSharingViolation,
        DiagnosticDescriptors.VSA030_FeatureCrossReferenceViolation
    );

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register namespace validation
        context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.FileScopedNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
        
        // Register type declaration validation
        context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration, 
            SyntaxKind.ClassDeclaration, 
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration);

        // Register compilation-level validation for request/response sharing
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
    {
        var syntaxTree = context.Node.SyntaxTree;
        var filePath = syntaxTree.FilePath;

        // Only analyze files in Features projects
        if (!FeaturePathHelper.IsFeaturesProject(filePath))
            return;

        var namespaceName = GetNamespaceName(context.Node);
        if (string.IsNullOrEmpty(namespaceName))
            return;

        // Validate namespace format
        if (!FeaturePathHelper.IsValidFeaturesNamespace(namespaceName, out var unitName, out var featureName))
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.VSA002_InvalidNamespace,
                context.Node.GetLocation(),
                namespaceName);
            context.ReportDiagnostic(diagnostic);
            return;
        }

        // Extract feature name from file path
        var expectedFeatureName = FeaturePathHelper.ExtractFeatureName(filePath);
        if (expectedFeatureName == null || unitName == null || featureName == null)
            return;

        // Validate that namespace matches file path
        if (!featureName.Equals(expectedFeatureName, StringComparison.Ordinal))
        {
            var expectedNamespace = FeaturePathHelper.BuildExpectedNamespace(unitName, expectedFeatureName);
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.VSA001_FeatureFolderNamespaceMismatch,
                context.Node.GetLocation(),
                namespaceName,
                expectedNamespace);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        var syntaxTree = context.Node.SyntaxTree;
        var filePath = syntaxTree.FilePath;

        // Only analyze files in Features projects
        if (!FeaturePathHelper.IsFeaturesProject(filePath))
            return;

        var typeDeclaration = context.Node as BaseTypeDeclarationSyntax;
        if (typeDeclaration == null)
            return;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);
        if (typeSymbol == null)
            return;

        // Only check public types
        if (typeSymbol.DeclaredAccessibility != Accessibility.Public)
            return;

        // Check if this is a UseCase implementation
        var isUseCase = IsUseCaseImplementation(typeSymbol, out var isDirectImplementation);
        
        if (isUseCase)
        {
            // Validate direct implementation
            if (!isDirectImplementation)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.VSA011_IndirectUseCaseImplementation,
                    typeDeclaration.GetLocation(),
                    typeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
            return; // UseCases are allowed to be public
        }

        // Check if this is a Request or Response type for a UseCase
        if (IsRequestOrResponseType(typeSymbol, context.Compilation))
            return; // Request/Response types are allowed to be public

        // If we reach here, this public type is not authorized
        var unauthorizedDiagnostic = Diagnostic.Create(
            DiagnosticDescriptors.VSA010_UnauthorizedPublicType,
            typeDeclaration.GetLocation(),
            typeSymbol.Name);
        context.ReportDiagnostic(unauthorizedDiagnostic);
    }

    private void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var compilation = context.Compilation;

        // Find all UseCases in the compilation
        var useCases = new List<(INamedTypeSymbol UseCase, ITypeSymbol Request, ITypeSymbol Response)>();

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (!FeaturePathHelper.IsFeaturesProject(syntaxTree.FilePath))
                continue;

            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            
            foreach (var classDecl in classDeclarations)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
                if (classSymbol == null)
                    continue;

                if (TryGetUseCaseTypes(classSymbol, out var requestType, out var responseType))
                {
                    useCases.Add((classSymbol, requestType, responseType));
                }
            }
        }

        // Check for Request/Response sharing violations
        CheckRequestResponseSharing(context, useCases);
    }

    private void CheckRequestResponseSharing(
        CompilationAnalysisContext context,
        List<(INamedTypeSymbol UseCase, ITypeSymbol Request, ITypeSymbol Response)> useCases)
    {
        var requestUsage = new Dictionary<ITypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
        var responseUsage = new Dictionary<ITypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);

        foreach (var (useCase, request, response) in useCases)
        {
            // Only track types from the same assembly
            if (SymbolEqualityComparer.Default.Equals(request.ContainingAssembly, context.Compilation.Assembly))
            {
                if (!requestUsage.ContainsKey(request))
                    requestUsage[request] = new List<INamedTypeSymbol>();
                requestUsage[request].Add(useCase);
            }

            if (SymbolEqualityComparer.Default.Equals(response.ContainingAssembly, context.Compilation.Assembly))
            {
                if (!responseUsage.ContainsKey(response))
                    responseUsage[response] = new List<INamedTypeSymbol>();
                responseUsage[response].Add(useCase);
            }
        }

        // Report sharing violations
        foreach (var kvp in requestUsage.Concat(responseUsage))
        {
            var type = kvp.Key;
            var useCaseList = kvp.Value;
            
            if (useCaseList.Count > 1)
            {
                foreach (var location in type.Locations)
                {
                    if (location.IsInSource)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptors.VSA020_RequestResponseSharingViolation,
                            location,
                            type.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }

    private static bool IsUseCaseImplementation(ITypeSymbol typeSymbol, out bool isDirectImplementation)
    {
        isDirectImplementation = false;

        if (typeSymbol is not INamedTypeSymbol namedType)
            return false;

        // Check direct interfaces
        foreach (var iface in namedType.Interfaces)
        {
            if (IsIUseCaseInterface(iface))
            {
                isDirectImplementation = true;
                return true;
            }
        }

        // Check if implemented through base class
        var baseType = namedType.BaseType;
        while (baseType != null)
        {
            foreach (var iface in baseType.Interfaces)
            {
                if (IsIUseCaseInterface(iface))
                {
                    isDirectImplementation = false;
                    return true;
                }
            }
            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool IsRequestOrResponseType(ITypeSymbol typeSymbol, Compilation compilation)
    {
        // Find all UseCases in the compilation
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            
            foreach (var classDecl in classDeclarations)
            {
                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
                if (classSymbol == null)
                    continue;

                if (TryGetUseCaseTypes(classSymbol, out var requestType, out var responseType))
                {
                    if (SymbolEqualityComparer.Default.Equals(typeSymbol, requestType) ||
                        SymbolEqualityComparer.Default.Equals(typeSymbol, responseType))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool TryGetUseCaseTypes(INamedTypeSymbol classSymbol, out ITypeSymbol requestType, out ITypeSymbol responseType)
    {
        requestType = null!;
        responseType = null!;

        foreach (var iface in classSymbol.Interfaces)
        {
            if (IsIUseCaseInterface(iface) && iface is INamedTypeSymbol namedInterface)
            {
                var typeArgs = namedInterface.TypeArguments;
                if (typeArgs.Length == 2)
                {
                    requestType = typeArgs[0];
                    responseType = typeArgs[1];
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsIUseCaseInterface(ITypeSymbol symbol)
    {
        if (symbol is not INamedTypeSymbol namedType)
            return false;

        return namedType.Name == "IUseCase" &&
               namedType.TypeArguments.Length == 2 &&
               namedType.ContainingNamespace?.ToString() == "VerticalSliceArchitecture.Core";
    }

    private static string GetNamespaceName(SyntaxNode node)
    {
        return node switch
        {
            FileScopedNamespaceDeclarationSyntax fileScopedNs => fileScopedNs.Name.ToString(),
            NamespaceDeclarationSyntax ns => ns.Name.ToString(),
            _ => string.Empty
        };
    }
}
