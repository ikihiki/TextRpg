# API シーケンスダイアグラム

本ドキュメントでは、TextRPG プラットフォームの gRPC API 呼び出しの関係性をシーケンスダイアグラムで示します。

## 目次

1. [システム概要](#システム概要)
2. [ゲーム開始フロー](#ゲーム開始フロー)
3. [プレイヤーアクション処理フロー](#プレイヤーアクション処理フロー)
4. [挿絵生成フロー](#挿絵生成フロー)
5. [ローカルゲートウェイ双方向ストリーミング](#ローカルゲートウェイ双方向ストリーミング)
6. [正史ノート管理フロー](#正史ノート管理フロー)

---

## システム概要

```
┌─────────────────┐
│  Web Frontend   │
│   (gRPC-Web)    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐      ┌─────────────────┐
│  BFF Gateway    │◄────►│  Core Backend   │
│  (GameApi)      │      │  (Session/Notes)│
└────────┬────────┘      └────────┬────────┘
         │                        │
         ▼                        ▼
┌─────────────────┐      ┌─────────────────┐
│ AI Orchestrator │      │  Jobs/Workers   │
│   (Router)      │      │  (Hangfire)     │
└────────┬────────┘      └────────┬────────┘
         │                        │
         └──────────┬─────────────┘
                    ▼
         ┌─────────────────┐
         │ Local Gateway   │
         │ (自宅PC/LLM)    │
         └─────────────────┘
```

---

## ゲーム開始フロー

プレイヤーが新しいゲームを開始する際の API 呼び出しシーケンスです。

```mermaid
sequenceDiagram
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway<br/>(GameApiService)
    participant Core as Core Backend<br/>(SessionService)
    participant AI as AI Orchestrator<br/>(OrchestratorService)

    Frontend->>BFF: StartGame(StartGameRequest)
    Note right of BFF: story_id, character, config

    BFF->>Core: CreateSession(CreateSessionRequest)
    Note right of Core: user_id, story_id,<br/>player_character, config
    Core-->>BFF: CreateSessionResponse(session)

    BFF->>AI: GenerateNarrative(GenerateNarrativeRequest)
    Note right of AI: context, action=null (opening)
    AI-->>BFF: GenerateNarrativeResponse
    Note left of AI: narrative, scene_description

    BFF->>AI: GenerateActionSuggestions(GenerateActionSuggestionsRequest)
    Note right of AI: context, num_suggestions
    AI-->>BFF: GenerateActionSuggestionsResponse
    Note left of AI: suggestions[]

    BFF-->>Frontend: StartGameResponse
    Note left of BFF: session, opening_narrative,<br/>suggested_actions[]
```

### 関連 RPC 一覧

| サービス | RPC | リクエスト | レスポンス |
|---------|-----|-----------|-----------|
| GameApiService | StartGame | StartGameRequest | StartGameResponse |
| SessionService | CreateSession | CreateSessionRequest | CreateSessionResponse |
| OrchestratorService | GenerateNarrative | GenerateNarrativeRequest | GenerateNarrativeResponse |
| OrchestratorService | GenerateActionSuggestions | GenerateActionSuggestionsRequest | GenerateActionSuggestionsResponse |

---

## プレイヤーアクション処理フロー

プレイヤーがアクションを実行した際の処理シーケンスです。

```mermaid
sequenceDiagram
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway<br/>(GameApiService)
    participant Core as Core Backend<br/>(SessionService)
    participant AI as AI Orchestrator<br/>(OrchestratorService)
    participant Jobs as Jobs Service<br/>(WorkerService)
    participant Notes as Core Backend<br/>(NotesService)

    Frontend->>BFF: SubmitAction(SubmitActionRequest)
    Note right of BFF: session_id, action

    BFF->>Core: GetSession(GetSessionRequest)
    Core-->>BFF: GetSessionResponse(session)

    BFF->>Core: ProcessAction(ProcessActionRequest)
    Note right of Core: session_id, action
    
    Note over Core: ルールエンジンで<br/>機械的判定実行<br/>(ダイス/戦闘)
    
    Core-->>BFF: ProcessActionResponse
    Note left of Core: state, mechanical_results[]

    BFF->>AI: GenerateNarrative(GenerateNarrativeRequest)
    Note right of AI: context, action,<br/>mechanical_results
    AI-->>BFF: GenerateNarrativeResponse
    Note left of AI: narrative, scene_description,<br/>suggest_illustration

    alt 挿絵生成トリガー
        BFF->>Jobs: EnqueueJob(EnqueueJobRequest)
        Note right of Jobs: JOB_TYPE_ILLUSTRATION
        Jobs-->>BFF: EnqueueJobResponse(job_id)
    end

    BFF->>AI: GenerateNoteProposals(GenerateNoteProposalsRequest)
    Note right of AI: context, recent_narrative,<br/>existing_notes
    AI-->>BFF: GenerateNoteProposalsResponse
    Note left of AI: proposals[]

    alt ノート提案がある場合
        BFF->>Notes: ProposeNoteDiff(ProposeNoteDiffRequest)
        Note right of Notes: session_id, proposal, source
        Notes-->>BFF: ProposeNoteDiffResponse
    end

    BFF->>AI: GenerateActionSuggestions(GenerateActionSuggestionsRequest)
    AI-->>BFF: GenerateActionSuggestionsResponse
    Note left of AI: suggestions[]

    BFF->>Core: UpdateSession(UpdateSessionRequest)
    Note right of Core: session_id, updated state
    Core-->>BFF: UpdateSessionResponse

    BFF-->>Frontend: SubmitActionResponse
    Note left of BFF: narrative, state, mechanics[],<br/>suggested_actions[],<br/>note_proposals[]
```

### アクションタイプ別処理

| アクションタイプ | 機械的処理 | AI処理 |
|----------------|-----------|--------|
| ACTION_TYPE_NARRATIVE | なし | 物語生成 |
| ACTION_TYPE_COMBAT | ダイス判定、ダメージ計算 | 戦闘描写文章化 |
| ACTION_TYPE_DIALOGUE | なし | 会話生成 |
| ACTION_TYPE_MOVEMENT | 位置更新 | 場面転換描写 |
| ACTION_TYPE_USE_ITEM | アイテム効果適用 | 使用結果描写 |
| ACTION_TYPE_SKILL | スキル判定 | スキル発動描写 |

---

## 挿絵生成フロー

挿絵（イラスト）の非同期生成フローです。

```mermaid
sequenceDiagram
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway<br/>(GameApiService)
    participant Jobs as Jobs Service<br/>(WorkerService /<br/>IllustrationJobService)
    participant AI as AI Orchestrator<br/>(OrchestratorService)
    participant Assets as Core Backend<br/>(AssetService)
    participant Local as Local Gateway<br/>(GatewayService)

    Frontend->>BFF: RequestIllustration(RequestIllustrationRequest)
    Note right of BFF: session_id, scene_description,<br/>priority

    BFF->>Assets: GetVisualCanon(GetVisualCanonRequest)
    Note right of Assets: session_id, character_id
    Assets-->>BFF: GetVisualCanonResponse
    Note left of Assets: visual_canon

    BFF->>AI: GenerateSceneDescription(GenerateSceneDescriptionRequest)
    Note right of AI: context, narrative,<br/>visual_canon
    AI-->>BFF: GenerateSceneDescriptionResponse
    Note left of AI: scene_description,<br/>art_style, key_elements

    BFF->>Jobs: EnqueueIllustration(EnqueueIllustrationRequest)
    Note right of Jobs: session_id, turn_number,<br/>scene_description,<br/>visual_canon[], style
    Jobs-->>BFF: EnqueueIllustrationResponse
    Note left of Jobs: job_id, estimated_time

    BFF-->>Frontend: RequestIllustrationResponse
    Note left of BFF: accepted, job_id,<br/>estimated_wait_seconds

    Note over Jobs: ジョブキュー処理開始

    alt クラウドAI使用
        Jobs->>Jobs: Cloud AI Provider呼び出し
    else ローカルAI使用
        Jobs->>Local: WorkStream(WorkRequest)
        Note right of Local: WORK_TYPE_IMAGE_GENERATION
        Local-->>Jobs: WorkStream(WorkResult)
        Note left of Local: image_data, metrics
    end

    Jobs->>Assets: UploadAsset(UploadAssetRequest)
    Note right of Assets: session_id, metadata, data
    Assets-->>Jobs: UploadAssetResponse
    Note left of Assets: asset

    Note over Frontend,Local: ポーリングまたは通知

    Frontend->>BFF: GetIllustrations(GetIllustrationsRequest)
    BFF->>Jobs: GetIllustrationResult(GetIllustrationResultRequest)
    Jobs-->>BFF: GetIllustrationResultResponse
    Note left of Jobs: status, asset_id,<br/>image_url, metadata
    BFF-->>Frontend: GetIllustrationsResponse
```

### ジョブ状態遷移

```
JOB_STATUS_QUEUED
       │
       ▼
JOB_STATUS_PROCESSING ◄── JOB_STATUS_WAITING_LOCAL
       │                        (ローカルGW待ち)
       │
       ├──────────────┬──────────────┐
       ▼              ▼              ▼
JOB_STATUS_COMPLETED  JOB_STATUS_FAILED  JOB_STATUS_CANCELLED
```

---

## ローカルゲートウェイ双方向ストリーミング

自宅PCで動作するローカルゲートウェイとサーバー間の双方向通信フローです。

```mermaid
sequenceDiagram
    participant Local as Local Gateway<br/>(自宅PC)
    participant Server as Server<br/>(GatewayService)
    participant Jobs as Jobs Service<br/>(WorkerService)
    participant LLM as LLMExecutor<br/>(ローカルLLM)
    participant Image as ImageExecutor<br/>(ローカル画像生成)

    Note over Local,Server: 接続確立フェーズ

    Local->>Server: RegisterGateway(RegisterGatewayRequest)
    Note right of Server: gateway_id, name,<br/>capabilities, system_info
    Server-->>Local: RegisterGatewayResponse
    Note left of Server: success, gateway_token,<br/>configuration

    Local->>Server: WorkStream (双方向ストリーム開始)
    Note over Local,Server: 双方向ストリーム確立

    loop ハートビート
        Local->>Server: Heartbeat(HeartbeatRequest)
        Note right of Server: gateway_id, status, load
        Server-->>Local: HeartbeatResponse
        Note left of Server: acknowledged,<br/>pending_work_count
    end

    Note over Local,Server: ワーク実行フェーズ

    Jobs->>Server: ローカル実行ジョブ発生
    Server->>Local: WorkStreamMessage(WorkRequest)
    Note left of Local: work_id, work_type,<br/>parameters, priority

    Local->>Server: WorkStreamMessage(Acknowledgment)
    Note right of Server: message_id, accepted

    alt LLM生成タスク
        Local->>LLM: Generate(LLMGenerateRequest)
        LLM-->>Local: LLMGenerateResponse
    else 画像生成タスク
        Local->>Image: Generate(ImageGenerateRequest)
        Image-->>Local: ImageGenerateResponse
    end

    opt 進捗報告
        Local->>Server: WorkStreamMessage(WorkProgress)
        Note right of Server: work_id, progress_percent,<br/>status_message
    end

    Local->>Server: WorkStreamMessage(WorkResult)
    Note right of Server: work_id, success,<br/>result, metrics

    Server->>Jobs: ジョブ完了通知
```

### ワークタイプと実行先

| WorkType | 説明 | 必要なCapability |
|----------|------|------------------|
| WORK_TYPE_LLM_GENERATION | テキスト生成 | llm_models |
| WORK_TYPE_IMAGE_GENERATION | 画像生成 | image_models |
| WORK_TYPE_EMBEDDING | 埋め込みベクトル生成 | llm_models |
| WORK_TYPE_IMAGE_ANALYSIS | 画像分析/キャプション | image_models |
| WORK_TYPE_VOICE_SYNTHESIS | 音声合成 | plugins |
| WORK_TYPE_PLUGIN | カスタムプラグイン | plugins |

### 制御メッセージ

```mermaid
sequenceDiagram
    participant Local as Local Gateway
    participant Server as Server

    Note over Local,Server: 一時停止
    Server->>Local: ControlMessage(CONTROL_TYPE_PAUSE)
    Local->>Server: Acknowledgment(accepted)

    Note over Local,Server: 再開
    Server->>Local: ControlMessage(CONTROL_TYPE_RESUME)
    Local->>Server: Acknowledgment(accepted)

    Note over Local,Server: 特定ワーク中止
    Server->>Local: ControlMessage(CONTROL_TYPE_CANCEL_WORK)
    Note left of Local: parameters: {work_id: "xxx"}
    Local->>Server: Acknowledgment(accepted)

    Note over Local,Server: 切断
    Local->>Server: ControlMessage(CONTROL_TYPE_DISCONNECT)
    Server->>Local: Acknowledgment(accepted)
    Note over Local,Server: ストリーム終了
```

---

## 正史ノート管理フロー

正史ノート（Canonical Notes）の管理フローです。AIが提案し、人間が確定します。

```mermaid
sequenceDiagram
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway<br/>(GameApiService)
    participant Notes as Core Backend<br/>(NotesService)
    participant AI as AI Orchestrator<br/>(OrchestratorService)

    Note over Frontend,AI: ノート提案フロー

    AI->>Notes: ProposeNoteDiff(ProposeNoteDiffRequest)
    Note right of Notes: session_id, proposal,<br/>source (AI model)
    Notes-->>AI: ProposeNoteDiffResponse
    Note left of Notes: proposal (with ID)

    Note over Frontend,AI: ノート確認フロー

    Frontend->>BFF: GetNotes(GetNotesRequest)
    Note right of BFF: session_id, category,<br/>page_size
    BFF->>Notes: ListNotes(ListNotesRequest)
    Notes-->>BFF: ListNotesResponse
    Note left of Notes: notes[], proposals[]
    BFF->>Notes: ListProposals(ListProposalsRequest)
    Notes-->>BFF: ListProposalsResponse
    BFF-->>Frontend: GetNotesResponse
    Note left of BFF: notes[], proposals[]

    Note over Frontend,AI: 提案承認フロー

    Frontend->>BFF: ResolveNoteProposal(ResolveNoteProposalRequest)
    Note right of BFF: session_id, proposal_id,<br/>accept, modification

    BFF->>Notes: ResolveProposal(ResolveProposalRequest)
    Note right of Notes: proposal_id, accept, reason

    alt 承認の場合
        Notes->>Notes: CreateNote または UpdateNote
        Notes-->>BFF: ResolveProposalResponse(note)
    else 却下の場合
        Notes-->>BFF: ResolveProposalResponse(null)
    end

    BFF-->>Frontend: ResolveNoteProposalResponse
    Note left of BFF: success, note

    Note over Frontend,AI: ノート手動更新フロー

    Frontend->>BFF: UpdateNote(UpdateNoteRequest)
    Note right of BFF: session_id, note_id, update
    BFF->>Notes: UpdateNote(UpdateNoteRequest)
    Notes-->>BFF: UpdateNoteResponse
    BFF-->>Frontend: UpdateNoteResponse
```

### ノートタイプと用途

| NoteType | 説明 | 用途 |
|----------|------|------|
| NOTE_TYPE_PIN | 不変のコア事実 | キャラクター名、重要設定など |
| NOTE_TYPE_ANCHOR | 安定性を提供する重要事実 | ストーリー上の重要イベント |
| NOTE_TYPE_THREAD | 物語スレッドを形成する関連事実 | 伏線、サブプロット |

### 確認ステータス

```
CONFIRMATION_STATUS_HYPOTHESIS (AI提案)
              │
              ▼
    ┌─────────┴─────────┐
    │                   │
    ▼                   ▼
CONFIRMATION_STATUS_CONFIRMED  (却下 → 削除)
    (人間確定)
              │
              ▼
CONFIRMATION_STATUS_DEPRECATED
    (後継ノートにより非推奨)
```

---

## API 呼び出し関係図（サマリー）

```mermaid
flowchart TB
    subgraph Frontend["Web Frontend"]
        F1[GameApiService Client]
    end

    subgraph BFF["BFF Gateway"]
        B1[GameApiService]
    end

    subgraph Core["Core Backend"]
        C1[SessionService]
        C2[NotesService]
        C3[AssetService]
    end

    subgraph AI["AI Orchestrator"]
        A1[OrchestratorService]
    end

    subgraph Jobs["Jobs Service"]
        J1[WorkerService]
        J2[IllustrationJobService]
        J3[ReportJobService]
    end

    subgraph Local["Local Gateway"]
        L1[GatewayService]
        L2[LLMExecutorService]
        L3[ImageExecutorService]
    end

    F1 -->|gRPC-Web| B1
    B1 -->|gRPC| C1
    B1 -->|gRPC| C2
    B1 -->|gRPC| C3
    B1 -->|gRPC| A1
    B1 -->|gRPC| J1
    B1 -->|gRPC| J2
    
    J1 -->|Bidirectional Stream| L1
    J2 -->|via WorkStream| L1
    
    A1 -.->|Cloud AI| CloudAI[(Cloud AI)]
    A1 -.->|via Jobs| J1
    
    L1 --> L2
    L1 --> L3
```

---

## 付録：プロトコルファイル一覧

| ディレクトリ | ファイル | 主なサービス |
|-------------|----------|-------------|
| proto/bff/ | game_api.proto | GameApiService |
| proto/core/ | session.proto | SessionService |
| proto/core/ | notes.proto | NotesService |
| proto/core/ | assets.proto | AssetService |
| proto/ai/ | orchestrator.proto | OrchestratorService |
| proto/jobs/ | worker.proto | WorkerService, IllustrationJobService, ReportJobService |
| proto/local/ | gateway.proto | GatewayService, LLMExecutorService, ImageExecutorService |
