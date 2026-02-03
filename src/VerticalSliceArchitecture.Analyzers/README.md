# Vertical Slice Architecture Analyzer

This Roslyn analyzer enforces architectural rules for `*.Features` projects in the TextRPG codebase.

## Purpose

The analyzer ensures:
- **Namespace consistency**: Namespaces must match the feature folder structure
- **Public API control**: Only UseCases and their Request/Response types can be public
- **Direct IUseCase implementation**: UseCases must implement `IUseCase<,>` directly, not through inheritance
- **Request/Response uniqueness**: Each UseCase must have unique Request/Response types (no sharing within the same assembly)
- **Feature isolation**: Features can only reference public UseCases and their Request/Response types from other features

## Diagnostic IDs

| ID | Severity | Description |
|----|----------|-------------|
| VSA001 | Error | Feature folder name and namespace mismatch |
| VSA002 | Error | Invalid namespace format |
| VSA010 | Error | Unauthorized public type |
| VSA011 | Error | Indirect IUseCase implementation |
| VSA020 | Error | Request/Response sharing violation |
| VSA030 | Error | Feature cross-reference violation |

## Expected Project Structure

```
/src
  /<Unit>
    <Unit>.Domain/
    <Unit>.Features/
      Features/
        <FeatureName>/
          <UseCase>.cs
          <UseCase>Request.cs
          <UseCase>Response.cs
```

## Usage

The analyzer is automatically applied to `*.Features` projects through project references:

```xml
<ProjectReference Include="..\..\VerticalSliceArchitecture.Analyzers\VerticalSliceArchitecture.Analyzers.csproj" 
                  OutputItemType="Analyzer" 
                  ReferenceOutputAssembly="false" />
```

## Example: Valid UseCase

```csharp
using VerticalSliceArchitecture.Core;

namespace Story.Features.ScenarioStart;

// ✅ Request type - can be public
public class StartScenarioRequest
{
    public string ScenarioId { get; set; } = string.Empty;
}

// ✅ Response type - can be public
public class StartScenarioResponse
{
    public bool Success { get; set; }
}

// ✅ UseCase implementing IUseCase directly - can be public
public class StartScenario : IUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse { Success = true };
    }
}

// ✅ Internal helper class - OK
internal class ScenarioHelper
{
    public void Help() { }
}
```

## Example: Common Violations

### VSA001: Namespace Mismatch
```csharp
// ❌ File is in Features/ScenarioStart/ but namespace says WrongName
namespace Story.Features.WrongName;

public class StartScenario { }
```

### VSA002: Invalid Namespace Format
```csharp
// ❌ Missing "Features" in namespace
namespace Story.ScenarioStart;

public class StartScenario { }
```

### VSA010: Unauthorized Public Type
```csharp
namespace Story.Features.ScenarioStart;

// ❌ This class is not a UseCase, Request, or Response
public class UnauthorizedClass
{
    public string Name { get; set; }
}
```

### VSA011: Indirect IUseCase Implementation
```csharp
// ❌ UseCase implements IUseCase through base class
public abstract class BaseUseCase<TReq, TRes> : IUseCase<TReq, TRes>
{
    public abstract TRes Execute(TReq request);
}

// ❌ This will trigger VSA011
public class StartScenario : BaseUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public override StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse();
    }
}
```

### VSA020: Request/Response Sharing
```csharp
// ❌ Shared Request type used by multiple UseCases in the same assembly
public class SharedRequest { }
public class SharedResponse { }

public class UseCase1 : IUseCase<SharedRequest, SharedResponse> { }
public class UseCase2 : IUseCase<SharedRequest, SharedResponse> { } // ❌ VSA020
```

## Configuration

The `.editorconfig` file in the repository root ensures all diagnostics are treated as errors:

```ini
[*.cs]
dotnet_diagnostic.VSA001.severity = error
dotnet_diagnostic.VSA002.severity = error
dotnet_diagnostic.VSA010.severity = error
dotnet_diagnostic.VSA011.severity = error
dotnet_diagnostic.VSA020.severity = error
dotnet_diagnostic.VSA030.severity = error
```

## Testing

The analyzer includes comprehensive unit tests. Run them with:

```bash
dotnet test src/VerticalSliceArchitecture.Analyzers.Tests/
```

## CI Integration

The analyzer is enforced in CI through the GitHub Actions workflow (`.github/workflows/build.yml`), which builds all `*.Features` projects with the analyzer enabled.
