# gRPC Proto Files Documentation

本ドキュメントは、TextRpg バックエンドの gRPC Protocol Buffers 定義について説明します。

## 概要

TextRpg プラットフォームは、以下のサービス境界で gRPC を使用しています：

1. **BFF Gateway** - Web フロントエンドとの gRPC-Web 通信
2. **Core Backend** - セッション・ノート・アセット管理
3. **AI Orchestrator** - AI による物語生成・提案
4. **Jobs Worker** - Hangfire による非同期ジョブ処理
5. **Local Gateway** - 自宅 PC との双方向ストリーミング

## ディレクトリ構成

```
proto/
├── bff/
│   └── game_api.proto          # フロントエンド向け API
├── core/
│   ├── common.proto            # Core Backend 共有 enum / 型
│   ├── session.proto           # セッション・状態管理
│   ├── notes.proto             # 正史ノート（Lorebook）
│   └── assets.proto            # アセット・挿絵管理
├── ai/
│   └── orchestrator.proto      # AI オーケストレーション
├── jobs/
│   └── worker.proto            # バックグラウンドジョブ
└── local/
    └── gateway.proto           # ローカル Gateway 連携
```

## サービス概要

### 1. BFF Gateway (`proto/bff/game_api.proto`)

**目的**: Web フロントエンドとの通信エンドポイント（gRPC-Web 終端）

**主要サービス**:
- `GameApi` - すべてのフロントエンド操作を集約

**主要機能**:
- シナリオ管理（作成、一覧、更新、AI 相談）
- セッション管理（作成、開始、再開、一時停止）
- ゲームプレイ（プレイヤー入力、巻き戻し、ログ参照）
- 主人公設定

**通信方式**: Unary RPC（gRPC-Web 対応）

### 2. Core Backend (`proto/core/*.proto`)

#### 2.1 Session Service (`session.proto`)

**目的**: セッションと状態の唯一の真実（Single Source of Truth）

**主要機能**:
- セッションライフサイクル管理
- セッション状態管理（楽観的ロック対応）
- ターン管理（巻き戻し・無効化対応）
- チャプター管理
- AI コンテキスト構築
- 主人公データ管理

**重要な設計**:
- `SessionState.state_data` は `google.protobuf.Struct` で拡張可能
- `Turn.is_valid` で巻き戻し時のターン無効化をサポート
- バージョン管理による楽観的ロック

#### 2.2 Notes Service (`notes.proto`)

**目的**: 正史ノート（Canonical Notes / Lorebook）の構造化管理

**主要機能**:
- ノート CRUD 操作
- ノートタイプ（人物、場所、アイテム、組織、イベント、ルール、伝承）
- Canon レベル管理（噂、暫定、確定）
- PIN / ANCHOR / THREADS による記憶管理
- AI によるノート提案
- ノート間のリンク・関連付け

**重要な設計**:
- `CanonLevel` で情報の信頼度を制御
- `is_pinned` (PIN) で AI コンテキストに常時含める
- `is_anchored` (ANCHOR) で不変の核心情報を保護
- `evidence_turn_ids` で根拠となるターンを記録
- `CharacterNoteData`, `LocationNoteData` でタイプ別の構造化データ

#### 2.3 Asset Service (`assets.proto`)

**目的**: アセット（挿絵、音声など）と Visual Canon の管理

**主要機能**:
- アセット CRUD 操作
- 挿絵生成リクエスト
- Visual Canon 管理（キャラクター・場所の一貫した見た目）
- 生成ステータス追跡

**重要な設計**:
- `VisualCanonEntry` で見た目の正史を保持
- `IllustrationRequest` で詳細な生成パラメータ指定
- `associated_turn_id` でターンとの関連付け

### 3. AI Orchestrator (`proto/ai/orchestrator.proto`)

**目的**: AI による物語生成・提案（状態は変更しない）

**主要機能**:
- Narrative 生成（イントロ、ゲームプレイ、補足説明）
- 提案生成（主人公、ノート、プロット、伝承）
- NPC 対話・プロフィール生成
- 要約・あらすじ生成
- プロバイダー（クラウド/ローカル）ヘルスチェック

**重要な設計**:
- AI は状態を直接変更せず、提案のみ行う
- `AIContext` で AI に渡すコンテキストを構造化
- `AIProviderInfo` でどの AI を使用したか追跡
- `AIDiscretionLevel` で AI の裁量度を制御

### 4. Jobs Worker (`proto/jobs/worker.proto`)

**目的**: Hangfire による非同期ジョブ処理

**主要機能**:
- 挿絵生成ジョブ
- レポート生成ジョブ（セッションログ、小説形式、統計など）
- ローカル実行ジョブ（自宅 PC へのブリッジ）
- カスタムジョブ
- ジョブステータス追跡・制御（キャンセル、リトライ）

**重要な設計**:
- `ExecutionTarget` でクラウド/ローカル/自動を選択
- `JobPriority` で優先度管理
- `JobStatus` で詳細な進捗管理
- 柔軟な `google.protobuf.Struct` ペイロード

### 5. Local Gateway (`proto/local/gateway.proto`)

**目的**: 自宅 PC との双方向ストリーミング（NAT 回避）

**主要機能**:
- 双方向ストリーミングによるジョブ処理
- Gateway 登録・ヘルスチェック
- 能力（LLM、画像生成、音声生成）の宣言
- ハートビート監視

**重要な設計**:
- `WorkStream` で双方向ストリーミング（自宅 PC から接続）
- `Capability` で自宅 PC が提供できる機能を宣言
- `WorkAssignment` でサーバからジョブを割当
- `WorkResult` / `WorkProgress` / `WorkError` で結果報告

**通信フロー**:
1. 自宅 PC が `WorkStream` で接続開始
2. サーバが `WorkAssignment` でジョブ割当
3. 自宅 PC が処理実行
4. `WorkProgress` で進捗報告
5. 完了時に `WorkResult` を返送

## データ拡張性

すべての主要メッセージに `google.protobuf.Struct` フィールドを用意し、プラグインによる拡張を可能にしています：

- `SessionState.extensions` - セッション状態の拡張
- `ProtagonistData.extensions` - 主人公データの拡張
- `NoteData.extensions` - ノートデータの拡張
- `AIContext.extensions` - AI コンテキストの拡張

## コード生成

### C# (.NET)

```bash
# 単一ファイル
protoc --proto_path=proto --csharp_out=src/Shared/Protos proto/bff/game_api.proto

# すべてのファイル
for proto in proto/**/*.proto; do
  protoc --proto_path=proto --csharp_out=src/Shared/Protos "$proto"
done
```

### TypeScript (フロントエンド)

```bash
# grpc-web プラグインを使用
protoc --proto_path=proto \
  --js_out=import_style=commonjs,binary:frontend/src/grpc \
  --grpc-web_out=import_style=typescript,mode=grpcwebtext:frontend/src/grpc \
  proto/bff/game_api.proto
```

## Protocol Buffers ベストプラクティス

本プロジェクトでは以下の慣例を採用しています：

1. **proto3 構文**を使用
2. **パッケージ名**: `textrpg.<サービス名>`
3. **C# 名前空間**: `TextRpg.<サービス名>`
4. **フィールド番号**: 削除されたフィールドは予約
5. **列挙型**: `_UNSPECIFIED` を 0 として定義
6. **タイムスタンプ**: `google.protobuf.Timestamp` を使用
7. **拡張性**: `google.protobuf.Struct` で柔軟なデータ構造
8. **ドキュメント**: すべてのサービス・メッセージにコメント

## 今後の拡張

以下の拡張が検討されています：

1. **認証・認可** - メタデータでの JWT トークン管理
2. **ストリーミング** - プログレス通知やリアルタイム更新
3. **バージョニング** - API バージョン管理
4. **エラーハンドリング** - 詳細なエラーコード体系
5. **ページネーション** - カーソルベースのページング

## 参考資料

- [Protocol Buffers Documentation](https://protobuf.dev/)
- [gRPC Documentation](https://grpc.io/docs/)
- [gRPC-Web](https://github.com/grpc/grpc-web)
- [.NET gRPC](https://learn.microsoft.com/aspnet/core/grpc/)
