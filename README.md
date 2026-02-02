# AI Text RPG Platform (Working Title)

AIとユーザーが共同で物語を進行・編集できる  
**WebベースのテキストRPG / インタラクティブフィクション基盤**です。

- 物語生成は AI
- 数値・戦闘・ダイスはプログラム
- 正史（記憶）は人間主導で管理
- AI実行環境はクラウド / ローカルを柔軟に切替
- すべて後付け拡張可能な設計

本リポジトリは **基盤アーキテクチャとサービス実装**を扱います。

## 目的と特徴

### 目的
- AI主導の物語生成で起きがちな
  - 記憶欠落
  - キャラクター崩壊
  - 数値の不正確さ
  を**構造で防ぐ**
- 「AIと遊ぶ」ではなく  
  **「AIと一緒に作品を作る体験」**を提供する

### 主な特徴
- 🧠 **正史ノート（Canonical Notes）**
  - PIN / ANCHOR / THREADS による記憶管理
  - AIは提案のみ、人間が確定
- 🎲 **数値処理の完全分離**
  - 戦闘・ダイス・判定はルールエンジンが決定論的に処理
- 🧩 **プラグイン前提設計**
  - 挿絵、統計、記録物生成などは後付け可能
- 🖼️ **挿絵の自動生成（頻度制御可能）**
- ☁️🏠 **AI実行環境の切替**
  - 高品質はクラウド
  - 機密・実験的な内容は自宅PC
- 🔁 **gRPC + 双方向ストリーミング**
  - ローカルAIはサーバイベントを待ち受け可能

## 技術スタック（確定事項）

### 全体
- **.NET Aspire**（ローカル/本番オーケストレーション）
- **gRPC / gRPC-Web**
- **C# (.NET 8+)**
- **TypeScript（フロントエンド）**

### バックエンド
- ASP.NET Core (gRPC)
- PostgreSQL（メインDB + Hangfire storage）
- Hangfire（非同期ジョブ）
- OpenTelemetry（ログ / トレース / メトリクス）

### フロントエンド
- gRPC-Web
- TypeScript + React系（詳細未確定）
- UIは **スキーマ駆動**（actions / panels / widgets）

### ローカル実行（自宅PC）
- .NET Worker
- 通常gRPC（HTTP/2）
- **双方向ストリーミング**でジョブ待受
- アウトバウンド接続のみ（NAT回避）

## アーキテクチャ概要

### デプロイ単位（初期構成）

```

┌─────────────────────┐
│ Web Frontend        │
│ (gRPC-Web)          │
└─────────▲───────────┘
│
┌─────────┴───────────┐
│ BFF / Edge Gateway  │  ← gRPC-Web終端
└─────────▲───────────┘
│ gRPC
┌─────────┴───────────┐
│ Core Backend        │  ← Session / Notes / State
└─────────▲───────────┘
│
┌─────────┴───────────┐
│ AI Orchestrator     │  ← Router / Context / Sanitizer
└─────────▲───────────┘
│
┌─────────┴───────────┐
│ Jobs / Workers      │  ← Hangfire
└─────────▲───────────┘
│ 双方向gRPC
┌─────────┴───────────┐
│ Local Gateway       │  ← 自宅PC（LLM/画像）
└─────────────────────┘

```

## コア設計方針

### 1. 単一の真実（Single Source of Truth）
- `state` は **Core Backend のみ**が更新
- AIは state を直接変更しない

### 2. AIの役割を制限
- AIは以下のみ担当
  - 物語描写
  - 行動候補生成
  - ノート差分提案
  - 戦闘ログの文章化
- 数値・確率・乱数は一切扱わない

### 3. 正史ノート中心設計
- 要約ではなく **差分コミット**
- 確定 / 仮説を分離
- evidence（根拠ログ）を持てる構造

### 4. 後付け拡張前提
- プラグイン方式
- `state.ext` / `notes.ext` を用意
- schema_version + migration

## 通信設計

### ブラウザ ↔ サーバ
- gRPC-Web（Unary中心）
- Pushは行わない

### サーバ ↔ ローカルGateway
- 通常gRPC（HTTP/2）
- **双方向ストリーミング**
- Local Gateway が常時接続してジョブを待つ

## 非同期処理（Hangfire）

### 使用用途
- 挿絵生成
- 記録物（レポート/小説）生成
- 統計・分析
- ローカルAI実行の橋渡し

### 方針
- 本線ターン処理は止めない
- 失敗してもリトライ可能
- ローカル接続が無い場合は待機 or フォールバック

## 挿絵生成

- 一定ターン間隔 + イベント重要度で生成
- Visual Canon（見た目の正史）を保持
- 非同期ジョブで生成
- クラウド / ローカル切替可能

---

## 今後の議論予定（GitHub Issue想定）

- [ ] gRPC `.proto` 定義（BFF / Core / AI / Jobs）
- [ ] 正史ノートのJSONスキーマ
- [ ] UIスキーマ（actions / panels / widgets）
- [ ] 双方向ストリームの再接続・再割当ルール
- [ ] Story登録フォーマット
- [ ] セキュリティ / 認証詳細
- [ ] 初期MVPの機能スコープ

## 開発方針

- **最初はまとめて、ホットな部分から分離**
- 破綻しないことを最優先
- AI品質は「構造」で担保する
- 後から足せる設計を優先する

## ライセンス

未定（検討中）

## リポジトリ構成

本リポジトリは **.NET Aspire を中心にしたマルチプロジェクト構成**を採用します。  
最初は「まとめて動く」ことを優先しつつ、**ホットな部分（AI / Jobs / Gateway）から独立デプロイできる**ように設計されています。

### Vertical Slice Architecture（垂直スライスアーキテクチャ）

各バックエンドプロジェクトは **Vertical Slice Architecture** を採用しています。

従来のレイヤードアーキテクチャでは、技術的関心事（Controllers / Services / Repositories）でコードを分割しますが、  
Vertical Slice では **機能・ユースケース単位**でコードを配置します。

**メリット**:
- 🎯 **高い凝集性**: 1つの機能に必要なコードがすべて同じ場所にある
- 🔗 **低い結合度**: 機能間の依存が少なく、変更の影響範囲が限定的
- 👥 **並行開発**: チームが独立して異なる機能を開発可能
- 📦 **独立テスト**: 機能単位でのテストが容易

**構成パターン**:
```
Features/
├─ <機能名>/
│  ├─ <ユースケース>/
│  │  ├─ <Command/Query>.cs    # リクエスト定義
│  │  ├─ <Handler>.cs          # ハンドラー実装
│  │  └─ <Validator>.cs        # バリデーション
│  └─ Common/                  # 機能内共有（エンティティ、リポジトリ）
```

```text
/
├─ README.md
├─ .editorconfig
├─ .gitignore
├─ docs/                      # 設計ドキュメント・議論メモ
│  ├─ architecture.md
│  ├─ grpc-protos.md
│  ├─ notes-schema.md
│  └─ decisions/              # ADR（Architecture Decision Record）
│
├─ proto/                     # gRPC定義（言語非依存の唯一の正）
│  ├─ bff/
│  │  └─ game_api.proto
│  ├─ core/
│  │  ├─ session.proto
│  │  ├─ notes.proto
│  │  └─ assets.proto
│  ├─ ai/
│  │  └─ orchestrator.proto
│  ├─ jobs/
│  │  └─ worker.proto
│  └─ local/
│     └─ gateway.proto
│
├─ src/
│  ├─ AppHost/                # .NET Aspire AppHost（開発用起動点）
│  │  └─ AppHost.csproj
│  │
│  ├─ Shared/
│  │  ├─ Contracts/           # 共有DTO・Enum・UIスキーマ
│  │  ├─ Protos/              # protoから生成されたC#コード
│  │  └─ Utils/               # 共通ユーティリティ
│  │
│  ├─ CoreBackend/            # Unit B: Core Backend（Stable）
│  │  ├─ CoreBackend.csproj
│  │  ├─ Features/            # Vertical Slice: 機能単位でコードを配置
│  │  │  ├─ Sessions/         # セッション管理機能
│  │  │  │  ├─ CreateSession/
│  │  │  │  │  ├─ CreateSessionCommand.cs
│  │  │  │  │  ├─ CreateSessionHandler.cs
│  │  │  │  │  └─ CreateSessionValidator.cs
│  │  │  │  ├─ GetSession/
│  │  │  │  │  ├─ GetSessionQuery.cs
│  │  │  │  │  └─ GetSessionHandler.cs
│  │  │  │  ├─ UpdateSessionState/
│  │  │  │  │  ├─ UpdateSessionStateCommand.cs
│  │  │  │  │  └─ UpdateSessionStateHandler.cs
│  │  │  │  └─ Common/
│  │  │  │     ├─ Session.cs
│  │  │  │     ├─ SessionState.cs
│  │  │  │     └─ ISessionRepository.cs
│  │  │  ├─ Notes/            # 正史ノート管理機能
│  │  │  │  ├─ CreateNote/
│  │  │  │  ├─ UpdateNote/
│  │  │  │  ├─ GetNotes/
│  │  │  │  └─ Common/
│  │  │  │     ├─ Note.cs
│  │  │  │     └─ INoteRepository.cs
│  │  │  ├─ Assets/           # アセット管理機能
│  │  │  │  ├─ UploadAsset/
│  │  │  │  ├─ GetAssets/
│  │  │  │  └─ Common/
│  │  │  │     ├─ Asset.cs
│  │  │  │     └─ IAssetRepository.cs
│  │  │  └─ Turns/            # ターン管理機能
│  │  │     ├─ CreateTurn/
│  │  │     ├─ RewindTurn/
│  │  │     └─ Common/
│  │  │        └─ Turn.cs
│  │  ├─ Infrastructure/      # 横断的関心事
│  │  │  ├─ Persistence/
│  │  │  │  ├─ AppDbContext.cs
│  │  │  │  └─ Migrations/
│  │  │  └─ Configuration/
│  │  └─ Services/            # gRPC サービスエンドポイント
│  │     ├─ SessionService.cs
│  │     ├─ NotesService.cs
│  │     └─ AssetService.cs
│  │
│  ├─ RulesEngine/            # Unit C: ルールエンジン（ライブラリ）
│  │  ├─ RulesEngine.csproj
│  │  ├─ Features/
│  │  │  ├─ Combat/           # 戦闘判定機能
│  │  │  │  ├─ ResolveCombat/
│  │  │  │  └─ Common/
│  │  │  └─ Dice/             # ダイスロール機能
│  │  │     ├─ RollDice/
│  │  │     └─ Common/
│  │  └─ Tests/
│  │
│  ├─ AIOrchestrator/         # Unit D: AI Orchestrator（Hot）
│  │  ├─ AIOrchestrator.csproj
│  │  ├─ Features/            # Vertical Slice: 機能単位でコードを配置
│  │  │  ├─ NarrativeGeneration/    # 物語生成機能
│  │  │  │  ├─ GenerateIntro/
│  │  │  │  │  ├─ GenerateIntroCommand.cs
│  │  │  │  │  └─ GenerateIntroHandler.cs
│  │  │  │  ├─ GenerateGameplay/
│  │  │  │  └─ Common/
│  │  │  │     └─ NarrativeResult.cs
│  │  │  ├─ Suggestions/      # 提案生成機能
│  │  │  │  ├─ SuggestNote/
│  │  │  │  ├─ SuggestAction/
│  │  │  │  └─ Common/
│  │  │  ├─ ContextBuilding/  # コンテキスト構築機能
│  │  │  │  ├─ BuildContext/
│  │  │  │  │  ├─ BuildContextCommand.cs
│  │  │  │  │  └─ BuildContextHandler.cs
│  │  │  │  └─ Sanitize/
│  │  │  │     └─ Sanitizer.cs
│  │  │  └─ Routing/          # プロバイダールーティング機能
│  │  │     ├─ RouteToProvider/
│  │  │     │  └─ PolicyRouter.cs
│  │  │     └─ Common/
│  │  │        ├─ CloudProvider.cs
│  │  │        └─ LocalProvider.cs
│  │  └─ Services/            # gRPC サービスエンドポイント
│  │     └─ OrchestratorService.cs
│  │
│  ├─ Jobs/                   # Unit E: Hangfire Worker / Plugins
│  │  ├─ Jobs.csproj
│  │  ├─ Features/            # Vertical Slice: 機能単位でコードを配置
│  │  │  ├─ Illustration/     # 挿絵生成機能
│  │  │  │  ├─ GenerateIllustration/
│  │  │  │  │  ├─ IllustrationJob.cs
│  │  │  │  │  └─ IllustrationJobHandler.cs
│  │  │  │  └─ Common/
│  │  │  ├─ Reports/          # レポート生成機能
│  │  │  │  ├─ GenerateSessionReport/
│  │  │  │  ├─ GenerateNovelExport/
│  │  │  │  └─ Common/
│  │  │  └─ LocalExecution/   # ローカル実行機能
│  │  │     ├─ ExecuteLocal/
│  │  │     │  └─ LocalExecutionJob.cs
│  │  │     └─ Common/
│  │  ├─ Infrastructure/
│  │  │  ├─ Hangfire/
│  │  │  └─ Plugins/
│  │  │     └─ PluginRuntime.cs
│  │  └─ Services/            # gRPC サービスエンドポイント
│  │
│  ├─ BffGateway/             # Unit A: gRPC-Web BFF
│  │  ├─ BffGateway.csproj
│  │  ├─ Features/            # Vertical Slice: 機能単位でコードを配置
│  │  │  ├─ Scenarios/        # シナリオ管理機能
│  │  │  │  ├─ CreateScenario/
│  │  │  │  ├─ ListScenarios/
│  │  │  │  └─ Common/
│  │  │  ├─ GameSessions/     # ゲームセッション機能
│  │  │  │  ├─ StartSession/
│  │  │  │  ├─ ResumeSession/
│  │  │  │  └─ Common/
│  │  │  └─ Gameplay/         # ゲームプレイ機能
│  │  │     ├─ SubmitAction/
│  │  │     ├─ RewindTurn/
│  │  │     └─ Common/
│  │  ├─ Infrastructure/
│  │  │  ├─ Auth/
│  │  │  └─ Middleware/
│  │  └─ Services/            # gRPC サービスエンドポイント
│  │     └─ GameApiService.cs
│  │
│  └─ LocalGateway/           # Unit F: 自宅PC側（別デプロイ）
│     ├─ LocalGateway.csproj
│     ├─ Features/            # Vertical Slice: 機能単位でコードを配置
│     │  ├─ LlmExecution/     # LLM実行機能
│     │  │  ├─ ExecuteLlm/
│     │  │  │  └─ LlmExecutor.cs
│     │  │  └─ Common/
│     │  └─ ImageGeneration/  # 画像生成機能
│     │     ├─ GenerateImage/
│     │     │  └─ ImageExecutor.cs
│     │     └─ Common/
│     ├─ Infrastructure/
│     │  ├─ Streaming/
│     │  │  └─ WorkStreamClient.cs
│     │  └─ Configuration/
│     └─ Services/            # gRPC サービスエンドポイント
│
├─ frontend/                  # フロントエンド（TypeScript）
│  ├─ package.json
│  ├─ src/
│  │  ├─ grpc/                # gRPC-Webクライアント
│  │  ├─ features/            # Vertical Slice: 機能単位でコードを配置
│  │  │  ├─ scenarios/
│  │  │  ├─ gameplay/
│  │  │  └─ notes/
│  │  ├─ ui/                  # 共有UIコンポーネント
│  │  │  ├─ ActionPanel.tsx
│  │  │  ├─ NotesPanel.tsx
│  │  │  └─ Illustration.tsx
│  │  └─ pages/
│  └─ public/
│
└─ scripts/                   # 開発・運用補助スクリプト
   ├─ db-init.sql
   ├─ local-gateway-run.ps1
   └─ migrate.ps1
```
