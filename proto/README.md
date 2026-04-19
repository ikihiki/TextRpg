# TextRpg Protocol Buffers

このディレクトリには、TextRpg バックエンドの gRPC サービス定義が含まれています。

## ディレクトリ構成

- **bff/** - BFF Gateway（Web フロントエンド向け gRPC-Web API）
- **core/** - Core Backend（セッション、ノート、アセット管理）
- **ai/** - AI Orchestrator（物語生成・提案）
- **jobs/** - Jobs Worker（Hangfire による非同期ジョブ処理）
- **local/** - Local Gateway（自宅 PC との双方向ストリーミング）

## ファイル一覧

| ファイル | 説明 | 主要サービス |
|---------|------|------------|
| `bff/game_api.proto` | フロントエンド API | GameApi |
| `core/common.proto` | Core 共有 enum / 型 | Core 共通 |
| `core/session.proto` | セッション管理 | SessionService |
| `core/notes.proto` | 正史ノート管理 | NotesService |
| `core/assets.proto` | アセット管理 | AssetService |
| `ai/orchestrator.proto` | AI オーケストレーション | OrchestratorService |
| `jobs/worker.proto` | バックグラウンドジョブ | WorkerService |
| `local/gateway.proto` | ローカル Gateway | GatewayService |

## コード生成

### C# (.NET)

`Shared.Protos` をビルドすると、すべての proto ファイルから C# コードが自動生成されます。

```bash
dotnet build src/Shared/Protos/Shared.Protos.csproj
```

### TypeScript (フロントエンド)

gRPC-Web 用に TypeScript コードを生成：

```bash
cd frontend
npm install
npm run proto
```

## 検証

Shared.Protos のビルドで proto の構文と C# 生成をまとめて検証できます。

```bash
dotnet build src/Shared/Protos/Shared.Protos.csproj
```

## ドキュメント

詳細なドキュメントは [docs/grpc-protos.md](../docs/grpc-protos.md) を参照してください。

## 統計

- **proto ファイル数**: 8 ファイル
- **合計行数**: 約 2,000 行
- **サービス数**: 7 サービス
- **メッセージ数**: 200+ メッセージ

## Protocol Buffers バージョン

すべてのファイルは **proto3** 構文を使用しています。

## 依存関係

標準的な Protocol Buffers 型のみを使用：

- `google.protobuf.Timestamp` - タイムスタンプ
- `google.protobuf.Struct` - 柔軟なデータ構造（拡張性のため）

## ライセンス

（検討中）
