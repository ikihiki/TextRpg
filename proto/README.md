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
| `core/session.proto` | セッション管理 | SessionService |
| `core/notes.proto` | 正史ノート管理 | NotesService |
| `core/assets.proto` | アセット管理 | AssetService |
| `ai/orchestrator.proto` | AI オーケストレーション | OrchestratorService |
| `jobs/worker.proto` | バックグラウンドジョブ | WorkerService |
| `local/gateway.proto` | ローカル Gateway | GatewayService |

## コード生成

### C# (.NET)

すべての proto ファイルから C# コードを生成：

```bash
# すべての proto ファイルをコンパイル
for proto in proto/**/*.proto; do
  protoc --proto_path=proto \
         --csharp_out=src/Shared/Protos \
         --grpc_out=src/Shared/Protos \
         --plugin=protoc-gen-grpc=$(which grpc_csharp_plugin) \
         "$proto"
done
```

### TypeScript (フロントエンド)

gRPC-Web 用に TypeScript コードを生成：

```bash
protoc --proto_path=proto \
       --js_out=import_style=commonjs,binary:frontend/src/grpc \
       --grpc-web_out=import_style=typescript,mode=grpcwebtext:frontend/src/grpc \
       proto/bff/game_api.proto
```

## 検証

すべての proto ファイルが正しい構文であることを確認：

```bash
for proto in proto/**/*.proto; do
  protoc --proto_path=proto --csharp_out=/tmp/proto_test "$proto" || echo "FAILED: $proto"
done
```

## ドキュメント

詳細なドキュメントは [docs/grpc-protos.md](../docs/grpc-protos.md) を参照してください。

## 統計

- **proto ファイル数**: 7 ファイル
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
