# Vertical Slice Architecture Analyzer

## 概要

本アナライザーは、`*.Features` プロジェクトにおいて、垂直スライスアーキテクチャ（Vertical Slice Architecture）の境界を Roslyn Analyzer により**ビルド時に強制**するものです。

## 目的

AI 主導の実装や将来的な規模拡大において、以下の問題を防ぐため、アーキテクチャルールをビルドエラーとして強制します：

- public 型の無秩序な増加
- Feature 間での内部実装参照（同一アセンブリ内で internal が見える問題）
- UseCase 規約の逸脱

## 対象スコープ

- **対象**: `*.Features` プロジェクト（例：`Story.Features`, `Battle.Features`）
- **非対象**: `*.Domain` プロジェクト（Domain は別アセンブリのため、本アナライザーでは対象外）

## プロジェクト構成

```
/src
  VerticalSliceArchitecture.Core/        # IUseCase インターフェース
  VerticalSliceArchitecture.Analyzers/   # Roslyn Analyzer 本体

/tests
  VerticalSliceArchitecture.Analyzers.Tests/  # アナライザーのユニットテスト
  Story/                                       # サンプルプロジェクト
    Story.Domain/
    Story.Features/
      Features/
        ScenarioStart/
          StartScenario.cs               # UseCase
          StartScenarioRequest.cs        # Request
          StartScenarioResponse.cs       # Response
  Battle/                                      # サンプルプロジェクト
    Battle.Domain/
    Battle.Features/
      Features/
        ResolveBattle/
          ResolveBattle.cs
          ResolveBattleRequest.cs
          ResolveBattleResponse.cs
```

## 診断ルール

すべての診断は **Error** として扱われ、ビルドを失敗させます。

### VSA001: Feature フォルダ名と namespace の不一致

**説明**: ファイルパスから抽出される Feature 名と、namespace が一致していません。

**期待される構造**:
- ファイルパス: `<ProjectRoot>/Features/<FeatureName>/.../*.cs`
- namespace: `<Unit>.Features.<FeatureName>`

**例**:

```csharp
// ❌ エラー: ファイルは Features/ScenarioStart/ 配下だが namespace が違う
// File: Story.Features/Features/ScenarioStart/StartScenario.cs
namespace Story.Features.WrongName;

public class StartScenario { }
```

```csharp
// ✅ 正しい
// File: Story.Features/Features/ScenarioStart/StartScenario.cs
namespace Story.Features.ScenarioStart;

public class StartScenario { }
```

### VSA002: 不正な namespace フォーマット

**説明**: namespace が `<Unit>.Features.<FeatureName>` のパターンに従っていません。

**例**:

```csharp
// ❌ エラー: "Features" が含まれていない
namespace Story.ScenarioStart;

public class StartScenario { }
```

```csharp
// ✅ 正しい
namespace Story.Features.ScenarioStart;

public class StartScenario { }
```

### VSA010: 許可されていない public 型

**説明**: UseCase とその Request/Response 以外の型が public として宣言されています。

**許可される public 型**:
1. `IUseCase<TRequest, TResponse>` を直接実装するクラス（UseCase）
2. その UseCase の `TRequest` 型
3. その UseCase の `TResponse` 型

**例**:

```csharp
namespace Story.Features.ScenarioStart;

// ❌ エラー: UseCase でも Request/Response でもない
public class Helper
{
    public void DoSomething() { }
}
```

```csharp
namespace Story.Features.ScenarioStart;

// ✅ 正しい: internal なら OK
internal class Helper
{
    public void DoSomething() { }
}
```

### VSA011: IUseCase の間接実装

**説明**: UseCase が `IUseCase<,>` を基底クラス経由で実装しています。直接実装が必要です。

**例**:

```csharp
// ❌ エラー: 基底クラス経由での実装
public abstract class BaseUseCase<TReq, TRes> : IUseCase<TReq, TRes>
{
    public abstract TRes Execute(TReq request);
}

public class StartScenario : BaseUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public override StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse();
    }
}
```

```csharp
// ✅ 正しい: 直接実装
public class StartScenario : IUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse();
    }
}
```

### VSA020: Request/Response 共有違反

**説明**: 同一アセンブリ内の複数の UseCase が、同じ Request 型または Response 型を使用しています。

**ルール**:
- 同一 Features アセンブリ内で定義された Request/Response 型は、1つの UseCase でのみ使用可能
- 他のアセンブリの型（例：Domain の型）は共有可能

**例**:

```csharp
// ❌ エラー: 同じ Request を複数の UseCase で使用
public class SharedRequest { }
public class Response1 { }
public class Response2 { }

public class UseCase1 : IUseCase<SharedRequest, Response1>
{
    public Response1 Execute(SharedRequest request) => new Response1();
}

public class UseCase2 : IUseCase<SharedRequest, Response2>  // ❌ VSA020
{
    public Response2 Execute(SharedRequest request) => new Response2();
}
```

```csharp
// ✅ 正しい: 各 UseCase が独自の Request/Response を持つ
public class Request1 { }
public class Response1 { }
public class Request2 { }
public class Response2 { }

public class UseCase1 : IUseCase<Request1, Response1>
{
    public Response1 Execute(Request1 request) => new Response1();
}

public class UseCase2 : IUseCase<Request2, Response2>
{
    public Response2 Execute(Request2 request) => new Response2();
}
```

### VSA030: Feature 間参照違反

**説明**: Feature が他の Feature の内部実装を参照しています。

**ルール**:
- Feature 間では、公開された UseCase とその Request/Response のみ参照可能
- 内部実装（internal 型）は参照不可

> **注**: VSA030 は現在定義のみで、実装は将来のバージョンで追加予定です。

## 使用方法

### プロジェクトへの適用

`*.Features` プロジェクトに以下の ProjectReference を追加します：

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\VerticalSliceArchitecture.Core\VerticalSliceArchitecture.Core.csproj" />
  <ProjectReference Include="..\..\src\VerticalSliceArchitecture.Analyzers\VerticalSliceArchitecture.Analyzers.csproj" 
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

### 正しい UseCase の例

```csharp
using VerticalSliceArchitecture.Core;

namespace Story.Features.ScenarioStart;

// Request 型 - public OK
public class StartScenarioRequest
{
    public string ScenarioId { get; set; } = string.Empty;
}

// Response 型 - public OK
public class StartScenarioResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

// UseCase - IUseCase を直接実装、public OK
public class StartScenario : IUseCase<StartScenarioRequest, StartScenarioResponse>
{
    public StartScenarioResponse Execute(StartScenarioRequest request)
    {
        return new StartScenarioResponse
        {
            Success = true,
            Message = $"Scenario {request.ScenarioId} started successfully"
        };
    }
}

// Helper クラス - internal なら OK
internal class ScenarioHelper
{
    public void ValidateScenario(string scenarioId)
    {
        // 内部処理
    }
}
```

## 設定

### .editorconfig

プロジェクトルートの `.editorconfig` でアナライザーのルールを Error として設定：

```ini
[*.cs]
# Vertical Slice Architecture Analyzer Rules - All are ERRORS
dotnet_diagnostic.VSA001.severity = error
dotnet_diagnostic.VSA002.severity = error
dotnet_diagnostic.VSA010.severity = error
dotnet_diagnostic.VSA011.severity = error
dotnet_diagnostic.VSA020.severity = error
dotnet_diagnostic.VSA030.severity = error
```

### CI/CD 統合

GitHub Actions ワークフロー (`.github/workflows/build.yml`) でアナライザーを自動実行：

```yaml
- name: Build
  run: dotnet build --configuration Release

- name: Run tests
  run: dotnet test --configuration Release

- name: Build Story.Features (enforces analyzer rules)
  run: dotnet build tests/Story/Story.Features/Story.Features.csproj --configuration Release

- name: Build Battle.Features (enforces analyzer rules)
  run: dotnet build tests/Battle/Battle.Features/Battle.Features.csproj --configuration Release
```

## テスト

アナライザーには包括的なユニットテストが含まれています：

```bash
# テストの実行
dotnet test tests/VerticalSliceArchitecture.Analyzers.Tests/

# 結果: 11個のテストすべてが成功
Passed!  - Failed: 0, Passed: 11, Skipped: 0, Total: 11
```

### テストのカバレッジ

- Feature パス抽出のテスト
- Namespace 検証のテスト
- Unit 名抽出のテスト
- エッジケース（Windows パス、ネストした namespace）のテスト

## 技術詳細

### アーキテクチャ

- **ターゲットフレームワーク**: netstandard2.0（Roslyn Analyzer の要件）
- **Roslyn バージョン**: 4.8.0
- **分析タイプ**:
  - ファイルレベル分析（VSA001, VSA002, VSA010, VSA011）
  - コンパイルレベル分析（VSA020）

### 主要コンポーネント

1. **FeaturesArchitectureAnalyzer**: メインのアナライザークラス
2. **DiagnosticDescriptors**: 診断定義
3. **FeaturePathHelper**: Feature 名やパスの抽出ユーティリティ

### パフォーマンス

- ファイルパスベースの Feature 抽出により高速動作
- 並行実行が有効（`EnableConcurrentExecution`）
- 生成コードは分析対象外（`GeneratedCodeAnalysisFlags.None`）

## トラブルシューティング

### ビルドエラーが発生する場合

1. **VSA001/VSA002 エラー**:
   - ファイルが `Features/<FeatureName>/` フォルダ内にあることを確認
   - namespace が `<Unit>.Features.<FeatureName>` パターンに一致することを確認

2. **VSA010 エラー**:
   - public にする必要がない型は `internal` に変更
   - UseCase、Request、Response のみを public に保つ

3. **VSA011 エラー**:
   - UseCase クラスで `IUseCase<,>` を直接実装
   - 基底クラス経由の実装は使用しない

4. **VSA020 エラー**:
   - 各 UseCase に固有の Request/Response 型を作成
   - 共有が必要な場合は Domain プロジェクトに移動

## まとめ

このアナライザーにより、`*.Features` プロジェクトのアーキテクチャ境界がビルド時に自動的に強制されます。これにより：

- ✅ public API の肥大化を防止
- ✅ Feature 間の不適切な結合を防止
- ✅ UseCase パターンの一貫性を保証
- ✅ チーム開発でのアーキテクチャ違反を早期検出

詳細な実装については、`src/VerticalSliceArchitecture.Analyzers/README.md` も参照してください。
