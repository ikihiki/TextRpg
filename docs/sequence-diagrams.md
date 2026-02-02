# シーケンスダイアグラム（ユーザーストーリー視点）

本ドキュメントは、AI Text RPG Platformの主要なユーザーストーリーを  
シーケンスダイアグラムとして可視化する。

各ダイアグラムは以下の観点を明確化する：
- **誰が**: 操作主体（プレイヤー、シナリオ作者、システム）
- **どんな目的で**: 達成したいゴール
- **どのような操作や入力を行い**: 具体的なアクション
- **その結果何が起きるのか**: システムの応答と状態変化

---

## 目次

1. [シナリオ登録フロー](#1-シナリオ登録フロー)
2. [セッション開始フロー](#2-セッション開始フロー)
3. [セッション進行（AI対話モード）フロー](#3-セッション進行ai対話モードフロー)
4. [セッション再開フロー](#4-セッション再開フロー)
5. [ノート（Lorebook）管理フロー](#5-ノートlorebook管理フロー)
6. [ノートの自動生成・通知フロー](#6-ノートの自動生成通知フロー)
7. [プログラム主導ナラティブフロー](#7-プログラム主導ナラティブフロー)
8. [モード遷移・例外系フロー](#8-モード遷移例外系フロー)
9. [シナリオ編集フロー](#9-シナリオ編集フロー)
10. [高度なシナリオ実行フロー](#10-高度なシナリオ実行フロー)

---

## 1. シナリオ登録フロー

### 観点
- **誰が**: シナリオ作者
- **目的**: 新しいシナリオを作成し、AIが一貫した物語を生成できる土台を作る
- **操作**: シナリオ作成画面でタイトル・ジャンル・世界観・AI設定・挿絵設定を入力
- **結果**: Draftシナリオが保存され、以降のセッションで使用可能になる

### シーケンスダイアグラム（US-01〜US-06, US-11〜US-22）

```mermaid
sequenceDiagram
    autonumber
    participant Author as シナリオ作者
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-01: シナリオ作成開始
    Author->>Frontend: 「新しいシナリオを作成」を選択
    Frontend->>BFF: CreateScenario(title, summary?)
    BFF->>Core: CreateScenario RPC
    Core->>DB: INSERT Scenario (Draft)
    DB-->>Core: ScenarioId
    Core-->>BFF: ScenarioId
    BFF-->>Frontend: ScenarioId
    Frontend-->>Author: シナリオ編集画面を表示

    %% US-02: ジャンル・雰囲気設定
    Author->>Frontend: ジャンル・トーンを入力
    Frontend->>BFF: UpdateScenarioSettings(genre, tone)
    BFF->>Core: UpdateScenarioSettings RPC
    Core->>DB: UPDATE ScenarioSetting
    DB-->>Core: OK
    Core-->>BFF: OK
    BFF-->>Frontend: 設定保存完了

    %% US-03: 世界観設定
    Author->>Frontend: 世界観（Lore）を入力
    Frontend->>BFF: UpdateLore(loreText)
    BFF->>Core: UpdateLore RPC
    Core->>DB: UPDATE Lore
    DB-->>Core: OK

    %% US-17: AI相談機能
    Author->>Frontend: 「AIに相談」を選択
    Frontend->>BFF: GetAISuggestion(context)
    BFF->>AI: GenerateSuggestion RPC
    AI->>AI: コンテキスト分析・提案生成
    AI-->>BFF: SuggestionResponse
    BFF-->>Frontend: 提案内容
    Frontend-->>Author: 編集可能な提案を表示
    Author->>Frontend: 提案を採用/編集/破棄

    %% US-04: AI裁量レベル設定
    Author->>Frontend: AI裁量レベルを選択
    Frontend->>BFF: UpdateAISettings(discretionLevel)
    BFF->>Core: UpdateAISettings RPC
    Core->>DB: UPDATE AIConfig
    DB-->>Core: OK

    %% US-11〜US-15: 挿絵設定
    Author->>Frontend: 挿絵テイスト・ムード・NG要素を入力
    Frontend->>BFF: UpdateIllustrationConfig(style, mood, negativePrompt)
    BFF->>Core: UpdateIllustrationConfig RPC
    Core->>DB: UPDATE IllustrationConfig
    DB-->>Core: OK

    %% US-14: 挿絵プレビュー
    Author->>Frontend: プレビュー生成を実行
    Frontend->>BFF: GenerateIllustrationPreview(sampleScene)
    BFF->>AI: GeneratePreviewImage RPC
    AI-->>BFF: PreviewImageUrl
    BFF-->>Frontend: プレビュー画像
    Frontend-->>Author: 挿絵プレビューを表示

    %% US-06: 開始シーン定義
    Author->>Frontend: 開始シーンを入力
    Frontend->>BFF: UpdateOpeningScene(sceneText)
    BFF->>Core: UpdateOpeningScene RPC
    Core->>DB: UPDATE OpeningScene
    DB-->>Core: OK

    %% 保存完了
    Frontend-->>Author: シナリオ保存完了（Draft状態）
```

---

## 2. セッション開始フロー

### 観点
- **誰が**: プレイヤー
- **目的**: 選択したシナリオから新しいセッションを開始し、物語体験を始める
- **操作**: シナリオ選択 → イントロ閲覧 → 主人公確定 → セッション開始
- **結果**: セッションがActive状態になり、本編の物語が開始される

### シーケンスダイアグラム（US-S01〜US-S05）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-S01: セッション作成
    Player->>Frontend: シナリオを選択し「新しいセッションを開始」
    Frontend->>BFF: CreateSession(scenarioId)
    BFF->>Core: CreateSession RPC
    Core->>DB: SELECT Scenario
    DB-->>Core: ScenarioData
    Core->>DB: INSERT Session (Preparing)
    Core->>DB: INSERT ScenarioSnapshot
    DB-->>Core: SessionId
    Core-->>BFF: SessionId, Status=Preparing
    BFF-->>Frontend: SessionId

    %% US-S02: イントロ表示
    Frontend->>BFF: GetSessionIntro(sessionId)
    BFF->>Core: GetSessionIntro RPC
    Core->>AI: GenerateIntroNarrative(lore, genre, tone, openingScene)
    AI->>AI: イントロ生成（主人公未確定表現）
    AI-->>Core: IntroNarrative
    Core-->>BFF: IntroNarrative
    BFF-->>Frontend: イントロテキスト
    Frontend-->>Player: イントロを表示

    %% US-S03: 主人公確定（パターン分岐）
    alt パターンA: キャラクター固定
        Core-->>Frontend: 固定キャラクター情報
        Frontend-->>Player: 固定キャラクターを表示（確認のみ）
    else パターンB: キャラクター選択式
        Core-->>Frontend: 候補キャラクターリスト
        Frontend-->>Player: キャラクター選択UIを表示
        Player->>Frontend: キャラクターを選択
    else パターンC: キャラクタークリエイト
        Frontend-->>Player: キャラクター作成フォームを表示
        Player->>Frontend: 名前・性別・年齢等を入力
    else パターンD: AIによる自動生成
        Player->>Frontend: 「AIに任せる」を選択
        Frontend->>BFF: GenerateProtagonist(sessionId)
        BFF->>AI: GenerateProtagonist RPC
        AI->>AI: シナリオ・イントロを踏まえて主人公案生成
        AI-->>BFF: ProtagonistProposal
        BFF-->>Frontend: 主人公案
        Frontend-->>Player: 主人公案を表示（確認・修正可能）
        Player->>Frontend: 確認・修正して確定
    end

    %% 主人公確定
    Player->>Frontend: 主人公を確定
    Frontend->>BFF: ConfirmProtagonist(sessionId, protagonistData)
    BFF->>Core: ConfirmProtagonist RPC
    Core->>DB: UPDATE Session.Protagonist
    DB-->>Core: OK
    Core-->>BFF: OK
    BFF-->>Frontend: 主人公確定完了

    %% US-S04: 最終確認
    Frontend-->>Player: 開始サマリーを表示
    Player->>Frontend: 内容を確認

    %% US-S05: セッション正式開始
    Player->>Frontend: 「物語を始める」を選択
    Frontend->>BFF: StartSession(sessionId)
    BFF->>Core: StartSession RPC
    Core->>DB: UPDATE Session.Status = Active
    Core->>AI: GenerateFirstNarrative(context)
    AI->>AI: 本編最初のNarrative生成
    AI-->>Core: FirstNarrative
    Core->>DB: INSERT Turn (narrative)
    DB-->>Core: TurnId
    Core-->>BFF: FirstNarrative, Status=Active
    BFF-->>Frontend: 本編開始
    Frontend-->>Player: 本編最初のNarrativeを表示
```

---

## 3. セッション進行（AI対話モード）フロー

### 観点
- **誰が**: プレイヤー
- **目的**: AIと自然言語で対話しながら物語を自由に進める
- **操作**: 自然言語で行動入力 → AI応答確認 → 次の行動入力...のループ
- **結果**: Turn単位で物語が進行し、セッションログが蓄積される

### シーケンスダイアグラム（US-P01〜US-P11）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-P01: 状況提示
    Note over Player, DB: セッション進行ループ

    loop AI対話ループ
        %% US-P02, US-P03: 行動入力と結果
        Player->>Frontend: 自然言語で行動を入力
        Frontend->>BFF: SubmitPlayerInput(sessionId, inputText)
        BFF->>Core: ProcessPlayerInput RPC
        
        Core->>DB: SELECT SessionState, RecentTurns, Lorebook
        DB-->>Core: Context Data
        
        Core->>AI: GenerateNarrative(context, playerInput)
        AI->>AI: 行動解釈・結果生成
        AI-->>Core: NarrativeResponse
        
        Core->>DB: INSERT Turn (playerInput, narrative)
        DB-->>Core: TurnId
        
        Core-->>BFF: NarrativeResponse, TurnId
        BFF-->>Frontend: Narrative
        Frontend-->>Player: 行動結果をNarrativeとして表示
    end

    %% US-P04: NPC会話
    Note over Player, AI: NPC会話も同様のフローで処理
    Player->>Frontend: NPCへの発言を入力
    Frontend->>BFF: SubmitPlayerInput(sessionId, dialogueText)
    BFF->>Core: ProcessPlayerInput RPC
    Core->>AI: GenerateNPCResponse(context, dialogue, npcProfile)
    AI->>AI: NPC立場・性格に沿った返答生成
    AI-->>Core: NPCDialogueNarrative
    Core->>DB: INSERT Turn
    Core-->>BFF: NPCDialogueNarrative
    BFF-->>Frontend: NPC会話結果
    Frontend-->>Player: NPC返答を表示

    %% US-P05: 補足説明要求
    Player->>Frontend: 「今の状況を簡単にまとめて」
    Frontend->>BFF: RequestClarification(sessionId, question)
    BFF->>Core: ProcessClarificationRequest RPC
    Core->>AI: GenerateClarification(context, question)
    AI-->>Core: ClarificationText
    Note over Core: 物語進行は変化しない
    Core-->>BFF: ClarificationText
    BFF-->>Frontend: 補足説明
    Frontend-->>Player: 補足説明を表示

    %% US-P07, US-P11: 巻き戻し
    Player->>Frontend: 「ここまで戻る」を選択（任意のターン）
    Frontend->>BFF: RewindToTurn(sessionId, targetTurnId)
    BFF->>Core: RewindSession RPC
    Core->>DB: UPDATE Turns (invalidate after targetTurnId)
    Core->>AI: RebuildContext(targetTurnId)
    AI-->>Core: RebuiltContext
    Core->>DB: UPDATE SessionState
    DB-->>Core: OK
    Core-->>BFF: RewindComplete
    BFF-->>Frontend: 巻き戻し完了
    Frontend-->>Player: 指定ターンから再開可能

    %% US-P09: TOCからのログ参照
    Player->>Frontend: TOCの見出しを選択
    Frontend->>Frontend: 該当ターンにジャンプ
    Frontend-->>Player: 対象ターンを強調表示（ReadOnly）
```

---

## 4. セッション再開フロー

### 観点
- **誰が**: プレイヤー
- **目的**: 中断したセッションを途中から再開し、続きを遊ぶ
- **操作**: セッション一覧から選択 → あらすじ確認 → 再開
- **結果**: AIコンテキストが復元され、最終状態から物語を継続できる

### シーケンスダイアグラム（US-R01〜US-R08）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-R01, US-R03: セッション選択と進行度確認
    Player->>Frontend: 中断セッション一覧を表示
    Frontend->>BFF: ListSessions(userId, status=Paused)
    BFF->>Core: ListSessions RPC
    Core->>DB: SELECT Sessions
    DB-->>Core: SessionList (with progress info)
    Core-->>BFF: SessionList
    BFF-->>Frontend: セッション一覧（ターン数・時間含む）
    Frontend-->>Player: 一覧表示（進行度付き）

    Player->>Frontend: 再開したいセッションを選択
    
    %% US-R02: あらすじ確認
    Frontend->>BFF: GetSessionSummary(sessionId)
    BFF->>Core: GetSessionSummary RPC
    Core->>DB: SELECT ChapterSummary, RecentTurns
    DB-->>Core: SummaryData
    Core->>AI: GenerateRecapSummary(summaryData)
    AI->>AI: あらすじ要約生成
    AI-->>Core: RecapSummary
    Core-->>BFF: RecapSummary
    BFF-->>Frontend: AI要約されたあらすじ
    Frontend-->>Player: あらすじを表示

    %% US-R05: 注意点確認
    Frontend->>BFF: GetSessionWarnings(sessionId)
    BFF->>Core: GetSessionWarnings RPC
    Core->>DB: SELECT ScenarioChanges, AIChanges
    DB-->>Core: ChangeLog
    Core-->>BFF: Warnings (変更点がある場合)
    BFF-->>Frontend: 注意事項
    Frontend-->>Player: Scenario/AI変更があれば表示

    %% US-R06: セッション再開
    Player->>Frontend: 「再開」を選択
    Frontend->>BFF: ResumeSession(sessionId)
    BFF->>Core: ResumeSession RPC
    
    %% US-R04: AIコンテキスト復元
    Core->>DB: SELECT Lorebook, State, ChapterSummary, RecentTurns
    DB-->>Core: ContextData
    Core->>AI: RebuildContext(contextData)
    AI->>AI: コンテキスト再構築
    AI-->>Core: ContextReady
    
    Core->>DB: UPDATE Session.Status = Active
    DB-->>Core: OK
    Core-->>BFF: ResumeComplete
    BFF-->>Frontend: 再開完了

    %% US-R07: 直前内容確認
    Frontend->>BFF: GetLastTurn(sessionId)
    BFF->>Core: GetLastTurn RPC
    Core->>DB: SELECT LastTurn
    DB-->>Core: LastTurnData
    Core-->>BFF: LastTurnData
    BFF-->>Frontend: 直前のNarrative
    Frontend-->>Player: 直前の展開を表示

    %% US-R08: ReadOnlyモード（再開せず閲覧のみ）
    alt 閲覧のみ
        Player->>Frontend: 「閲覧のみ」を選択
        Frontend->>BFF: GetSessionLog(sessionId, readOnly=true)
        BFF->>Core: GetSessionLog RPC
        Core->>DB: SELECT AllTurns
        DB-->>Core: TurnLog
        Core-->>BFF: TurnLog
        BFF-->>Frontend: 全ログ（ReadOnly）
        Frontend-->>Player: ReadOnlyモードで表示
    end
```

---

## 5. ノート（Lorebook）管理フロー

### 観点
- **誰が**: プレイヤー
- **目的**: 人物・場所等の詳細情報を構造化して管理し、AIのブレ防止とトークン削減に活用
- **操作**: ノート新規作成 → 詳細入力 → 確定度設定 → 保存
- **結果**: Lorebookが構築され、Narrative生成時に参照される

### シーケンスダイアグラム（US-L01〜US-L10）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-L01: 人物ノート作成
    Player->>Frontend: 「人物」ノートを新規作成
    Frontend-->>Player: 人物ノートフォームを表示
    Player->>Frontend: プロフィール項目を入力
    Note over Player: 表示名、外見、口調、性格、<br/>能力、関係性、現在状態等
    
    %% US-L03: 確定度設定
    Player->>Frontend: 各フィールドの確定度を設定
    Note over Frontend: Canon / 未確定 / 噂
    
    Frontend->>BFF: CreateNote(sessionId, noteType=Person, data)
    BFF->>Core: CreateNote RPC
    Core->>DB: INSERT Note (Person)
    DB-->>Core: NoteId
    Core-->>BFF: NoteId
    BFF-->>Frontend: ノート保存完了
    Frontend-->>Player: 保存完了を表示

    %% US-L02: 場所ノート作成
    Player->>Frontend: 「場所」ノートを新規作成
    Frontend-->>Player: 場所ノートフォームを表示
    Player->>Frontend: 位置づけ・雰囲気・危険度等を入力
    Frontend->>BFF: CreateNote(sessionId, noteType=Location, data)
    BFF->>Core: CreateNote RPC
    Core->>DB: INSERT Note (Location)
    DB-->>Core: NoteId
    Core-->>BFF: NoteId
    BFF-->>Frontend: ノート保存完了

    %% US-L04: AIがLorebookを参照
    Note over Player, AI: Narrative生成時のLorebook参照
    Frontend->>BFF: SubmitPlayerInput(sessionId, inputText)
    BFF->>Core: ProcessPlayerInput RPC
    Core->>DB: SELECT RelatedNotes (by mention, location)
    DB-->>Core: RelevantLorebook
    Core->>AI: GenerateNarrative(context, lorebook, playerInput)
    AI->>AI: Canon情報を優先して矛盾なく生成
    AI-->>Core: Narrative
    Core-->>BFF: Narrative
    BFF-->>Frontend: AI応答
    Frontend-->>Player: Narrative表示

    %% US-L05: 矛盾検出と確認
    AI->>AI: 矛盾検出
    alt 矛盾あり
        AI-->>Core: ConflictDetected(noteId, conflictDetail)
        Core-->>BFF: ConflictWarning
        BFF-->>Frontend: 矛盾警告
        Frontend-->>Player: 矛盾を確認するUIを表示
        Player->>Frontend: 解決方法を選択
        Note over Player: ノート更新 / AI出力修正 / 噂として保持
        Frontend->>BFF: ResolveConflict(resolution)
        BFF->>Core: ResolveConflict RPC
        Core->>DB: UPDATE Note or Turn
        Core-->>BFF: OK
        BFF-->>Frontend: 解決完了
    end

    %% US-L07, US-L08: コンテキスト圧縮と章要約
    Player->>Frontend: 「章要約を生成」を実行
    Frontend->>BFF: GenerateChapterSummary(sessionId, chapterRange)
    BFF->>Core: GenerateChapterSummary RPC
    Core->>DB: SELECT Turns (chapterRange)
    DB-->>Core: ChapterTurns
    Core->>AI: SummarizeChapter(turns, lorebook)
    AI->>AI: 章要約生成
    AI-->>Core: ChapterSummary
    Core->>DB: INSERT ChapterSummary
    DB-->>Core: OK
    Core-->>BFF: ChapterSummary
    BFF-->>Frontend: 章要約完了
    Frontend-->>Player: 章要約を表示

    %% US-L09: 参照ノート可視化
    Note over Frontend: 各ターンで参照されたノート一覧を表示
    Frontend-->>Player: 参照ノートリンク（クリックでノート編集へ）

    %% US-L10: 整合性チェック
    Player->>Frontend: 「整合性チェック」を実行
    Frontend->>BFF: CheckConsistency(sessionId)
    BFF->>Core: CheckConsistency RPC
    Core->>AI: ValidateLorebook(notes, summaries)
    AI->>AI: 矛盾検出
    AI-->>Core: InconsistencyList
    Core-->>BFF: InconsistencyList
    BFF-->>Frontend: 矛盾候補リスト
    Frontend-->>Player: 矛盾候補を表示（修正はユーザー判断）
```

---

## 6. ノートの自動生成・通知フロー

### 観点
- **誰が**: システム（AI主導）、プレイヤー（確認・採用）
- **目的**: AIが重要情報を検出してノート案を自動生成し、ユーザーに確認を求める
- **操作**: 通知確認 → 差分レビュー → 採用/却下/保留
- **結果**: 手動入力の手間を減らしつつ、ノートが育成される

### シーケンスダイアグラム（US-AN01〜US-AN07）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant Jobs as Hangfire Jobs
    participant DB as Database

    %% US-AN01, US-AN02: AI自動検出とノート生成
    Note over Core, Jobs: ターン確定後の非同期処理
    
    Core->>Jobs: EnqueueNoteExtraction(turnId)
    Jobs->>DB: SELECT Turn, ExistingNotes
    DB-->>Jobs: TurnData, Notes
    Jobs->>AI: ExtractEntitiesFromTurn(turnText, existingNotes)
    AI->>AI: エンティティ抽出（人物/場所/組織等）
    AI-->>Jobs: ExtractedEntities
    
    alt 新規エンティティ検出
        Jobs->>AI: ProposeNewNote(entity, turnId)
        AI->>AI: ノート案生成
        AI-->>Jobs: NoteProposal (Pending)
        Jobs->>DB: INSERT NoteUpdateProposal (Pending)
        DB-->>Jobs: ProposalId
    else 既存ノート更新
        Jobs->>AI: ProposeNoteUpdate(noteId, newInfo, turnId)
        AI->>AI: 更新差分生成
        AI-->>Jobs: UpdateProposal (before/after, evidence)
        Jobs->>DB: INSERT NoteUpdateProposal (Pending)
        DB-->>Jobs: ProposalId
    end
    
    %% US-AN03: 通知作成
    Jobs->>DB: INSERT Notification (proposalId, type, importance)
    DB-->>Jobs: NotificationId

    %% US-AN03: 通知表示
    Frontend->>BFF: GetNotifications(sessionId)
    BFF->>Core: GetNotifications RPC
    Core->>DB: SELECT Notifications (unread)
    DB-->>Core: NotificationList
    Core-->>BFF: NotificationList
    BFF-->>Frontend: 通知一覧
    Frontend-->>Player: 通知バッジ・一覧表示

    %% US-AN04: 差分レビューと採用/却下
    Player->>Frontend: 通知を開く
    Frontend->>BFF: GetProposalDetail(proposalId)
    BFF->>Core: GetProposalDetail RPC
    Core->>DB: SELECT NoteUpdateProposal
    DB-->>Core: ProposalDetail (before/after, evidence)
    Core-->>BFF: ProposalDetail
    BFF-->>Frontend: 差分ビュー
    Frontend-->>Player: 差分を表示（変更フィールド、根拠ターン）

    alt 採用 (Apply)
        Player->>Frontend: 「採用」を選択
        Frontend->>BFF: ApplyProposal(proposalId)
        BFF->>Core: ApplyProposal RPC
        Core->>DB: UPDATE Note (apply changes)
        Core->>DB: UPDATE Proposal.Status = Applied
        Core->>DB: UPDATE Notification.Read = true
        DB-->>Core: OK
        Core-->>BFF: OK
        BFF-->>Frontend: 採用完了
        Frontend-->>Player: ノート更新完了を表示
    else 一部採用 (Edit then Apply)
        Player->>Frontend: 内容を編集して「採用」
        Frontend->>BFF: ApplyProposal(proposalId, editedData)
        BFF->>Core: ApplyProposal RPC
        Core->>DB: UPDATE Note (edited changes)
        Core-->>BFF: OK
    else 却下 (Reject)
        Player->>Frontend: 「却下」を選択
        Frontend->>BFF: RejectProposal(proposalId, reason?)
        BFF->>Core: RejectProposal RPC
        Core->>DB: UPDATE Proposal.Status = Rejected
        Core-->>BFF: OK
    else 保留 (Snooze)
        Player->>Frontend: 「保留」を選択
        Frontend->>BFF: SnoozeProposal(proposalId)
        BFF->>Core: SnoozeProposal RPC
        Core->>DB: UPDATE Notification.Snoozed = true
        Core-->>BFF: OK
    end

    %% US-AN05: 通知設定
    Player->>Frontend: 通知設定を変更
    Frontend->>BFF: UpdateNotificationSettings(settings)
    Note over Frontend: 頻度、対象、自動採用ポリシー
    BFF->>Core: UpdateNotificationSettings RPC
    Core->>DB: UPDATE UserSettings
    Core-->>BFF: OK
    BFF-->>Frontend: 設定保存完了

    %% US-AN06: 矛盾検出通知
    Jobs->>AI: DetectConflicts(proposedChange, existingCanon)
    AI->>AI: 矛盾分析
    alt 矛盾あり
        AI-->>Jobs: ConflictDetected
        Jobs->>DB: INSERT Notification (type=ConflictWarning, high importance)
        Jobs-->>Frontend: 矛盾検出通知
        Frontend-->>Player: 「判断が必要」通知を表示
    end

    %% US-AN07: 要約更新
    Jobs->>AI: UpdateSessionSummary(notes, recentTurns)
    AI->>AI: 章要約/状態要約更新
    AI-->>Jobs: UpdatedSummary
    Jobs->>DB: UPDATE ChapterSummary
    DB-->>Jobs: OK
```

---

## 7. プログラム主導ナラティブフロー

### 観点
- **誰が**: システム（プログラム）、プレイヤー（選択入力）
- **目的**: 拡張機能が定義したルールに基づいてプログラムで確実に処理し、公平性と再現性を保つ
- **操作**: 拡張機能が設定したUI要素による入力 → プログラム判定 → AI演出生成
- **結果**: ルールに基づいた結果が確定し、AIは演出のみを担当する

### 設計方針
- **拡張可能なアーキテクチャ**: プログラム主導モードの具体的な機能（バトル、判定、イベント等）は拡張機能として実装される
- **UI要素の動的構成**: 拡張機能がUI要素（ボタン、選択肢、表示パネル等）を定義し、Core/BFF経由でFrontendに配信する
- **ユーザー入力のルーティング**: ユーザー入力はCore経由で拡張機能が処理ロジックを担当する
- **汎用インターフェース**: Core/BFF/Frontendは拡張機能に依存しない汎用APIを提供する

### シーケンスダイアグラム（US-PG01〜US-PG10）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant Ext as Extension (拡張機能)
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-PG01: プログラム主導モード開始
    Note over Core, Ext: 拡張機能がモード開始をトリガー
    Ext->>Core: RequestModeChange(extensionId, modeConfig)
    Core->>DB: UPDATE Session.Mode = ProgramDriven
    Core->>DB: UPDATE Session.ActiveExtension = extensionId
    Core-->>BFF: ModeChange(ProgramDriven, extensionId, uiSchema)
    BFF-->>Frontend: ModeChange通知 + UI定義
    Frontend->>Frontend: 自由入力を無効化
    Frontend->>Frontend: 拡張機能が定義したUIを構築
    Frontend-->>Player: 拡張機能のモード名・UI表示

    %% US-PG02, US-PG03: 拡張機能主導の進行ループ
    Note over Player, Ext: 拡張機能が定義した進行ループ
    loop 拡張機能の進行単位
        Ext->>Core: UpdateUIRequest(uiElements)
        Note over Ext: 拡張機能がボタン・選択肢・表示を定義
        Core-->>BFF: UIUpdate(uiElements)
        BFF-->>Frontend: UI更新通知
        Frontend-->>Player: 拡張機能が設定したUIを表示
        Player->>Frontend: UI要素を操作（ボタン押下、選択等）
        Frontend->>BFF: SubmitExtensionInput(sessionId, extensionId, inputData)
        BFF->>Core: ProcessExtensionInput RPC
        
        Core->>Ext: HandleInput(inputData, sessionState)
        Ext->>Ext: 拡張機能固有のロジック実行
        Ext-->>Core: ExtensionResult (stateChanges, outcome)
        
        Core->>DB: UPDATE SessionState (拡張機能が指定した変更)
        DB-->>Core: OK
        
        %% US-PG07: AI演出生成
        Core->>AI: GenerateNarrative(extensionResult, context)
        AI->>AI: 結果を元に描写・心情・演出生成
        Note over AI: AIは結果を変更しない
        AI-->>Core: Narrative
        
        Core->>DB: INSERT Turn (input, result, narrative)
        Core-->>BFF: Narrative, CurrentState
        BFF-->>Frontend: 結果と演出
        Frontend-->>Player: 拡張機能が定義した形式で結果表示
    end

    %% US-PG04, US-PG05: 拡張機能による判定処理
    Note over Player, Ext: 拡張機能が定義した判定シーン
    Ext->>Core: RequestCheckUI(checkConfig)
    Core-->>BFF: ShowCheckUI(checkConfig)
    BFF-->>Frontend: 判定UI定義
    Frontend-->>Player: 判定UIを表示
    Player->>Frontend: 判定アクションを実行
    Frontend->>BFF: SubmitExtensionInput(sessionId, extensionId, checkAction)
    BFF->>Core: ProcessExtensionInput RPC
    
    Core->>Ext: ExecuteCheck(checkAction, modifiers)
    Ext->>Ext: 拡張機能固有の判定ロジック
    Ext-->>Core: CheckResult (outcome, details)
    
    Core->>DB: UPDATE SessionState (based on result)
    
    %% 分岐処理
    alt 成功
        Core->>AI: GenerateOutcomeNarrative(context, success=true)
        AI-->>Core: SuccessNarrative
    else 失敗
        Core->>AI: GenerateOutcomeNarrative(context, success=false)
        AI-->>Core: FailureNarrative
    end
    
    Core->>DB: INSERT Turn
    Core-->>BFF: CheckResult, Narrative
    BFF-->>Frontend: 判定結果と演出
    Frontend-->>Player: 拡張機能が定義した形式で結果表示

    %% US-PG06: 強制イベント（拡張機能主導）
    Note over Core, Ext: 拡張機能が強制イベントを発火
    Ext->>Core: TriggerForcedEvent(eventConfig)
    Core->>DB: UPDATE Session.Mode = ForcedEvent
    Core-->>BFF: ForcedEventStart(extensionId, eventType)
    BFF-->>Frontend: 強制イベント開始通知
    Frontend->>Frontend: 自由入力・分岐選択を非表示
    Frontend-->>Player: 「制御不能な状況」を明示
    
    loop イベント進行
        Ext->>Core: AdvanceEvent(eventStep)
        Core->>AI: GenerateEventNarrative(eventStep, context)
        AI-->>Core: EventNarrative
        Core->>DB: INSERT Turn
        Core-->>BFF: EventNarrative
        BFF-->>Frontend: イベント演出
        Frontend-->>Player: 自動再生表示
    end

    %% US-PG08, US-PG09: モード復帰
    Ext->>Core: RequestModeReturn()
    Core->>DB: UPDATE Session.Mode = AIDialogue
    Core->>DB: UPDATE Session.ActiveExtension = null
    Core-->>BFF: ModeChange(AIDialogue)
    BFF-->>Frontend: モード復帰通知
    Frontend->>Frontend: 自由入力を再有効化
    Frontend-->>Player: 「対話中」モード表示、入力欄復活

    %% US-PG10: テスト実行
    Note over Player: シナリオ作者によるテスト
    Player->>Frontend: 拡張機能シーンをテスト実行
    Frontend->>BFF: TestExtensionScene(extensionId, sceneId, testConfig)
    BFF->>Core: TestScene RPC
    Core->>Ext: ExecuteTest(sceneId, testConfig)
    Ext-->>Core: TestResult
    Core->>AI: GenerateNarrative(testResult, context)
    AI-->>Core: TestNarrative
    Core-->>BFF: TestResult, Narrative
    BFF-->>Frontend: テスト結果
    Frontend-->>Player: テスト実行結果を表示
```

---

## 8. モード遷移・例外系フロー

### 観点
- **誰が**: システム、プレイヤー
- **目的**: AI対話モードとプログラム主導モードを安全に切り替え、エラーや中断からも復帰できる
- **操作**: システム自動遷移 / エラー発生時の復帰操作
- **結果**: 状態不整合を防ぎ、セッションが破損しない

### シーケンスダイアグラム（US-M01〜US-M08）

```mermaid
sequenceDiagram
    autonumber
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant DB as Database

    %% US-M01, US-M02: モード切替
    Note over Core: バトル/判定/強制イベントトリガー
    Core->>DB: UPDATE Session.Mode, Session.ModeReason
    Core->>DB: INSERT ModeTransitionLog (reason, timestamp)
    DB-->>Core: OK
    Core-->>BFF: ModeChange(newMode, reason)
    BFF-->>Frontend: ModeChange通知
    
    Frontend->>Frontend: UIをモードに応じて切替
    alt AI対話モード
        Frontend-->>Player: 自由入力有効、「対話中」表示
    else プログラム主導モード
        Frontend-->>Player: 自由入力無効、モード名表示（バトル中/判定中/イベント進行中）
        Frontend-->>Player: 入力不可の理由を明示
    end

    %% US-M03: 正常終了からの復帰
    Note over Core: プログラム主導シーン完了
    Core->>DB: UPDATE Session.Mode = AIDialogue
    Core->>DB: INSERT ModeTransitionLog (return, timestamp)
    Core-->>BFF: ModeChange(AIDialogue)
    BFF-->>Frontend: モード復帰
    Frontend->>Frontend: 自由入力を再有効化
    Frontend-->>Player: 復帰後の状況をNarrativeで提示

    %% US-M04: エラー発生時の安全復帰
    Note over Core: プログラム主導モード中にエラー発生
    Core->>Core: エラー検出
    Core->>DB: SELECT LastCommittedState
    DB-->>Core: LastSafeState
    Core->>DB: ROLLBACK uncommitted changes
    Core->>DB: INSERT ErrorLog (error, uncommittedActions)
    Core-->>BFF: Error(errorDetail, lastSafePoint)
    BFF-->>Frontend: エラー通知
    Frontend-->>Player: エラー表示（どこまで確定したか明示）
    Frontend-->>Player: 「再試行」または「安全な地点に戻る」選択肢
    
    alt 再試行
        Player->>Frontend: 「再試行」を選択
        Frontend->>BFF: RetryFromLastSafe(sessionId)
        BFF->>Core: RetryFromLastSafe RPC
        Core->>DB: SELECT LastSafeState
        Core-->>BFF: RetryReady
        BFF-->>Frontend: 再試行開始
    else 安全な地点に戻る
        Player->>Frontend: 「安全な地点に戻る」を選択
        Frontend->>BFF: RewindToSafe(sessionId)
        BFF->>Core: RewindToSafe RPC
        Core->>DB: UPDATE Session to LastSafeState
        Core-->>BFF: RewindComplete
        BFF-->>Frontend: 復帰完了
    end

    %% US-M05: 通信断・画面離脱からの再開
    Note over Frontend: 通信断または画面離脱
    Frontend->>Frontend: 接続断検知
    Note over Player: 再接続
    Frontend->>BFF: Reconnect(sessionId)
    BFF->>Core: GetCurrentState RPC
    Core->>DB: SELECT Session, LastCommittedTurn, CurrentMode
    DB-->>Core: SessionState
    Core-->>BFF: SessionState (mode, lastTurn, pendingActions)
    BFF-->>Frontend: 復元データ
    Frontend->>Frontend: モード状態を復元
    alt 未完了処理あり
        Frontend-->>Player: 未完了の処理を再提示
    end
    Frontend-->>Player: 最後に確定した地点から再開

    %% US-M06: 巻き戻し制限
    Note over Player: プログラム主導モード中に巻き戻し試行
    Player->>Frontend: 「ここまで戻る」を選択
    Frontend->>Frontend: モード判定
    alt プログラム主導モード中
        Frontend-->>Player: 巻き戻しUI無効化
        Frontend-->>Player: 「終了後に可能」メッセージ表示
    else AI対話モード
        Frontend-->>Player: 巻き戻しUI有効
    end

    %% US-M07: モード遷移ログ
    Note over Core: 全モード遷移をログに記録
    Core->>DB: INSERT ModeTransitionLog
    Note over DB: 遷移理由、開始/終了時刻を記録

    %% US-M08: 強制進行中の情報確認
    Note over Player: 強制進行中
    Player->>Frontend: 状況確認ボタンを押す
    Frontend->>BFF: GetProgressInfo(sessionId)
    BFF->>Core: GetProgressInfo RPC
    Core->>DB: SELECT CurrentObjective, ProcessingStatus
    DB-->>Core: ProgressInfo
    Core-->>BFF: ProgressInfo
    BFF-->>Frontend: 進行情報
    Frontend-->>Player: 現在の目的・処理中内容を表示
```

---

## 9. シナリオ編集フロー

### 観点
- **誰が**: シナリオ作者
- **目的**: 既存シナリオを編集・改善し、品質を向上させる
- **操作**: シナリオ選択 → 編集 → AIチェック → プレビュー → 保存/公開
- **結果**: シナリオが更新され、新規セッションに反映される（既存セッションは影響なし）

### シーケンスダイアグラム（US-E01〜US-E10）

```mermaid
sequenceDiagram
    autonumber
    participant Author as シナリオ作者
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-E01: シナリオ選択と編集開始
    Author->>Frontend: 自分のシナリオ一覧を表示
    Frontend->>BFF: ListMyScenarios(userId)
    BFF->>Core: ListScenarios RPC
    Core->>DB: SELECT Scenarios (ownerId = userId)
    DB-->>Core: ScenarioList
    Core-->>BFF: ScenarioList
    BFF-->>Frontend: シナリオ一覧
    Frontend-->>Author: 一覧表示
    
    Author->>Frontend: 編集したいシナリオを選択
    Author->>Frontend: 「編集」を選択
    Frontend->>BFF: GetScenarioForEdit(scenarioId)
    BFF->>Core: GetScenario RPC
    Core->>DB: SELECT Scenario (full data)
    DB-->>Core: ScenarioData
    Core-->>BFF: ScenarioData
    BFF-->>Frontend: シナリオデータ
    Frontend-->>Author: シナリオ編集画面を表示

    %% US-E02: 基本情報編集
    Author->>Frontend: タイトル・概要を編集
    Frontend->>BFF: UpdateScenarioBasic(scenarioId, title, summary)
    BFF->>Core: UpdateScenarioBasic RPC
    Core->>DB: UPDATE Scenario (as draft)
    DB-->>Core: OK
    Core-->>BFF: OK
    BFF-->>Frontend: 保存完了

    %% US-E03: 世界観・トーン編集
    Author->>Frontend: ジャンル・トーン・Loreを編集
    Frontend->>BFF: UpdateScenarioSettings(scenarioId, settings)
    BFF->>Core: UpdateScenarioSettings RPC
    Core->>DB: UPDATE ScenarioSetting, Lore
    DB-->>Core: OK
    
    %% US-E04: AI設定編集
    Author->>Frontend: AI裁量レベル・Narrative方針を編集
    Frontend->>BFF: UpdateAIConfig(scenarioId, aiConfig)
    BFF->>Core: UpdateAIConfig RPC
    Core->>DB: UPDATE AIConfig
    DB-->>Core: OK

    %% US-E05: 挿絵設定編集
    Author->>Frontend: 挿絵テイスト・ムード・NG要素を編集
    Frontend->>BFF: UpdateIllustrationConfig(scenarioId, illustConfig)
    BFF->>Core: UpdateIllustrationConfig RPC
    Core->>DB: UPDATE IllustrationConfig
    DB-->>Core: OK

    %% US-E06: AIチェック
    Author->>Frontend: 「AIにチェック」を選択
    Frontend->>BFF: RequestAIReview(scenarioId)
    BFF->>Core: RequestAIReview RPC
    Core->>DB: SELECT Scenario (current draft)
    DB-->>Core: ScenarioData
    Core->>AI: ReviewScenario(scenarioData)
    AI->>AI: 矛盾検出・改善提案生成
    AI-->>Core: ReviewResult (issues, suggestions)
    Core-->>BFF: ReviewResult
    BFF-->>Frontend: レビュー結果
    Frontend-->>Author: 矛盾点・改善案を表示
    Note over Author: 自動確定は行われない

    %% US-E07: プレビュー
    Author->>Frontend: 「プレビュー」を実行
    Frontend->>BFF: PreviewScenario(scenarioId)
    BFF->>Core: PreviewScenario RPC
    Core->>AI: GeneratePreviewSession(scenarioData)
    AI->>AI: 仮セッション（イントロ・序盤）生成
    AI-->>Core: PreviewNarratives
    Core-->>BFF: PreviewNarratives
    BFF-->>Frontend: プレビュー内容
    Frontend-->>Author: 仮セッションを表示
    Note over Author: 本番セッションには影響しない

    %% US-E08: 保存
    Author->>Frontend: 「保存」を選択
    Frontend->>BFF: SaveScenario(scenarioId)
    BFF->>Core: SaveScenario RPC
    Core->>DB: UPDATE Scenario (commit draft)
    Core->>DB: INSERT EditHistory (timestamp, changeSummary)
    DB-->>Core: OK
    Core-->>BFF: OK
    BFF-->>Frontend: 保存完了
    Frontend-->>Author: 保存完了を表示

    %% US-E09: 公開
    Author->>Frontend: 公開状態を変更
    Frontend->>BFF: UpdateScenarioVisibility(scenarioId, isPublic)
    BFF->>Core: UpdateScenarioVisibility RPC
    Core->>DB: UPDATE Scenario.IsPublic
    DB-->>Core: OK
    Core-->>BFF: OK
    BFF-->>Frontend: 公開状態更新完了
    Frontend-->>Author: 公開状態を表示
    Note over Author: 新規セッションには最新版が使われる<br/>既存セッションは影響を受けない

    %% US-E10: 編集履歴確認
    Author->>Frontend: 「編集履歴」を選択
    Frontend->>BFF: GetEditHistory(scenarioId)
    BFF->>Core: GetEditHistory RPC
    Core->>DB: SELECT EditHistory
    DB-->>Core: HistoryList
    Core-->>BFF: HistoryList
    BFF-->>Frontend: 編集履歴
    Frontend-->>Author: 編集日時・変更概要を一覧表示
```

---

## 10. 高度なシナリオ実行フロー

### 観点
- **誰が**: シナリオ作者（設計時）、システム（実行時）、プレイヤー（体験時）
- **目的**: 候補プール・進行制御・非公開情報を使ってAIを監督し、長編・伏線回収が可能な物語を実現
- **操作**: シナリオ設計 → セッション実行 → AI軌道修正 → デバッグ確認
- **結果**: 予測可能で再現性のある物語進行が実現される

### シーケンスダイアグラム（US-AS01〜US-AS12）

```mermaid
sequenceDiagram
    autonumber
    participant Author as シナリオ作者
    participant Player as プレイヤー
    participant Frontend as Web Frontend
    participant BFF as BFF Gateway
    participant Core as Core Backend
    participant AI as AI Orchestrator
    participant DB as Database

    %% US-AS01, US-AS02: 候補プール定義
    Note over Author: シナリオ設計フェーズ
    Author->>Frontend: 人物候補（Cast）を登録
    Frontend->>BFF: AddCastMember(scenarioId, castData)
    Note over Frontend: 役割、口調、性格、秘密、登場条件
    BFF->>Core: AddCastMember RPC
    Core->>DB: INSERT CastPool
    DB-->>Core: CastId
    Core-->>BFF: OK
    BFF-->>Frontend: 登録完了
    
    Author->>Frontend: 場所候補（Location）を登録
    Frontend->>BFF: AddLocation(scenarioId, locationData)
    Note over Frontend: 雰囲気、危険度、関連人物、アクセス条件
    BFF->>Core: AddLocation RPC
    Core->>DB: INSERT LocationPool
    DB-->>Core: LocationId

    %% US-AS03, US-AS04: 章・ビート定義
    Author->>Frontend: Chapter/Beatを定義
    Frontend->>BFF: DefineChapterStructure(scenarioId, chapters)
    BFF->>Core: DefineChapterStructure RPC
    Core->>DB: INSERT Chapters, Beats
    Note over DB: Entry条件、Exit条件、禁止事項を含む
    DB-->>Core: OK

    %% US-AS05, US-AS06: 裏要約（HiddenBrief）定義
    Author->>Frontend: HiddenBriefを編集
    Frontend->>BFF: UpdateHiddenBrief(scenarioId, hiddenBrief)
    Note over Frontend: 秘密、真相、裏目的、伏線
    BFF->>Core: UpdateHiddenBrief RPC
    Core->>DB: INSERT/UPDATE HiddenBrief
    DB-->>Core: OK
    
    Author->>Frontend: 各秘密に公開条件を設定
    Frontend->>BFF: SetRevealConditions(secretId, conditions)
    Note over Frontend: フラグ、関係値、章進行
    BFF->>Core: SetRevealConditions RPC
    Core->>DB: UPDATE Secret.RevealConditions
    DB-->>Core: OK

    %% US-AS10: 条件付き強制イベント定義
    Author->>Frontend: トリガー条件とイベント内容を定義
    Frontend->>BFF: DefineForcedEvent(scenarioId, trigger, event)
    BFF->>Core: DefineForcedEvent RPC
    Core->>DB: INSERT ForcedEvent
    DB-->>Core: OK

    %% セッション実行フェーズ
    Note over Player, AI: セッション実行フェーズ

    %% AI参照と制御
    Player->>Frontend: 行動を入力
    Frontend->>BFF: SubmitPlayerInput(sessionId, input)
    BFF->>Core: ProcessPlayerInput RPC
    
    Core->>DB: SELECT CastPool, LocationPool, CurrentChapter, Beat, HiddenBrief
    DB-->>Core: ControlData
    
    Core->>AI: GenerateNarrative(context, controlData, hiddenBrief)
    Note over AI: 候補プールから選択<br/>Beat条件を認識<br/>HiddenBriefを前提に行動
    AI->>AI: 生成（禁止事項を遵守）
    AI-->>Core: Narrative

    %% US-AS07, US-AS08: 軌道修正・補完
    AI->>AI: 逸脱検出
    alt プレイヤーが脱線
        AI->>AI: 誘導イベント生成
        Note over AI: 情報不足を補う出会い<br/>候補人物・場所を優先使用
        AI-->>Core: CorrectionNarrative
    end
    
    AI->>AI: 手がかり不足検出
    alt 詰み状態の恐れ
        AI->>AI: 補完イベント生成
        Note over AI: NPCの助言、偶発イベント
        AI-->>Core: HintNarrative
    end

    %% US-AS06: 秘密の公開判定
    Core->>DB: SELECT SecretRevealConditions
    DB-->>Core: Conditions
    Core->>Core: 条件達成チェック
    alt 条件達成
        Core->>AI: 秘密を明示的に扱ってよい
    else 条件未達
        Core->>AI: 秘密は示唆止まりに
    end

    Core->>DB: INSERT Turn
    Core-->>BFF: Narrative
    BFF-->>Frontend: 応答
    Frontend-->>Player: 物語を表示

    %% US-AS09: 進行状態確認（作者向け）
    Note over Author: デバッグ/確認フェーズ
    Author->>Frontend: 進行状態を確認
    Frontend->>BFF: GetProgressState(sessionId)
    BFF->>Core: GetProgressState RPC
    Core->>DB: SELECT CurrentChapter, Beat, UnmetConditions
    DB-->>Core: ProgressState
    Core-->>BFF: ProgressState
    BFF-->>Frontend: 進行状態
    Frontend-->>Author: 現在のChapter/Beat/未達条件を表示
    Note over Author: 通常プレイヤーUIでは非表示

    %% US-AS12: AI参照情報確認（デバッグ）
    Author->>Frontend: AI参照情報を確認
    Frontend->>BFF: GetAIReferenceDebug(sessionId)
    BFF->>Core: GetAIReferenceDebug RPC
    Core->>DB: SELECT HiddenBrief, Canon, CurrentBeatContext
    DB-->>Core: DebugData
    Core-->>BFF: DebugData
    BFF-->>Frontend: デバッグ情報
    Frontend-->>Author: AIが参照している情報を表示
    Note over Author: プレイヤー向けUIでは使用不可

    %% US-AS11: テスト実行
    Author->>Frontend: 任意のChapter/Beatからテスト開始
    Frontend->>BFF: StartTestSession(scenarioId, startPoint, presetConditions)
    BFF->>Core: StartTestSession RPC
    Core->>DB: INSERT TestSession (with preset state)
    Note over Core: 指定Chapter/Beatを現在地に<br/>条件は満たした扱いに設定可能
    Core-->>BFF: TestSessionId
    BFF-->>Frontend: テストセッション開始
    Frontend-->>Author: 指定地点からテストプレイ可能
```

---

## 付録: システムコンポーネント凡例

本ドキュメントで使用するシステムコンポーネントは以下の通り：

| コンポーネント | 説明 |
|--------------|------|
| **Web Frontend** | TypeScript + React系のブラウザアプリケーション |
| **BFF Gateway** | gRPC-Web終端、認証・認可を担当 |
| **Core Backend** | Session/Notes/State管理、Single Source of Truth |
| **AI Orchestrator** | AI Router/Context Builder/Sanitizer |
| **Extension (拡張機能)** | プラグイン形式の拡張機能。UI要素定義・入力処理・ルール実行を担当 |
| **Hangfire Jobs** | 非同期ジョブ（挿絵生成、ノート抽出、要約更新） |
| **Database** | PostgreSQL（メインDB） |
| **Local Gateway** | 自宅PC側のローカルAI実行環境（オプション） |

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2024-XX-XX | 初版作成 |

---

## 関連ドキュメント

- [README.md](../README.md) - プロジェクト概要
- [docs/user-stories/](./user-stories/) - ユーザーストーリー詳細
- [docs/architecture.md](./architecture.md) - アーキテクチャ詳細（予定）
- [docs/grpc-protos.md](./grpc-protos.md) - gRPC定義（予定）
