# CONTRIBUTING.md
## TextRpg – Implementation Rules for GitHub Copilot

このリポジトリでは **GitHub Copilot による実装支援を前提**としています。  
Copilot が設計を破壊しないよう、**人間と AI の責務分担**を明確に定義します。

このドキュメントは **実装時に必ず遵守**してください。

---

## 1. 最重要ルール（破ったら差し戻し）

### 1.1 依存方向（絶対）
**依存は常に内向き**

Domain
↑
Application
↑
Adapters (Grpc)
↑
Host

**禁止事項**
- `Domain` / `Application` が gRPC / `.proto` / DTO を参照すること
- `Domain` が `Application` を参照すること
- gRPC 由来の型・例外を Application に流すこと

---

## 2. レイヤー別ルール

### 2.1 Domain
**許可**
- エンティティ
- 値オブジェクト
- ドメインサービス
- ドメインルール

**禁止**
- `.proto`
- gRPC / ASP.NET
- DTO
- Infrastructure 依存

👉 **純粋なビジネスロジックのみ**

---

### 2.2 Application
**責務**
- ユースケース実装
- ポート（インターフェース）定義

**ルール**
- 通信手段を知らない
- gRPC の存在を知らない
- `RpcException` を投げない
- 戻り値は `Result<T, Error>` 形式
⸻

2.3 Contracts（.proto）

ルール
	•	後方互換を維持する
	•	フィールド番号を再利用しない
	•	破壊的変更は禁止

禁止
	•	.proto 生成型を Domain / Application に渡すこと

⸻

2.4 Adapters.Grpc

唯一 DTO を扱ってよいレイヤー

責務
	•	gRPC Service 実装
	•	Application 呼び出し
	•	DTO ↔ Domain 変換
	•	エラー変換

Mapping ルール（重要）
	•	Mapping は Mapping フォルダに集約
	•	変換は 純粋関数
	•	他レイヤーで DTO を使わない

Adapters.Grpc
 ├─ Services
 └─ Mapping


⸻

2.5 Host

責務
	•	ASP.NET 起動
	•	DI 設定
	•	Hangfire / DB / 設定管理

注意
	•	ビジネスロジックを書かない
	•	Application を直接呼ぶ（初期フェーズ）

⸻

3. エラー・例外ルール

3.1 内部（Domain / Application）
	•	例外は原則使わない
	•	Result<T, Error> または限定されたドメイン例外のみ

3.2 gRPC 境界
	•	Result → StatusCode に変換
	•	Application に gRPC 例外を流さない

禁止
	•	Application から RpcException を throw

⸻

4. Copilot に実装させてよい単位

Copilot は 以下の作業に限定して使うこと。

OK
	•	Application のユースケース実装
	•	Domain ロジック
	•	Mapping クラス
	•	gRPC Service の薄い委譲コード
	•	テストコード（特に Mapping / ユースケース）

NG
	•	アーキテクチャ判断
	•	依存方向の変更
	•	レイヤーを跨ぐ型の共有
	•	.proto の設計判断

⸻

5. Copilot への指示テンプレ（推奨）

Copilot Chat / Inline に渡す際は、必ず以下を前提条件として伝えること。

- This project follows Clean Architecture.
- Domain and Application must not reference gRPC or proto types.
- DTOs are only allowed in Adapters.Grpc.
- Mapping must be implemented as pure functions.
- Application returns Result<T, Error>, not exceptions.


⸻

6. テストに関するルール

必須テスト
	•	Mapping のラウンドトリップ
	•	Application ユースケースの正常系 / 異常系

推奨
	•	Domain の不変条件テスト
	•	gRPC Service の薄い統合テスト

⸻

7. よくある Copilot 事故（禁止例）
	•	.proto の型を Domain に直接渡す
	•	gRPC DTO を Entity のプロパティにする
	•	Mapping を Service 内にベタ書き
	•	Application で RpcException を catch / throw
	•	Host にビジネスロジックを書く

→ すべて差し戻し対象

⸻

8. 判断に迷ったら

実装を止めて人間が決めること
	•	境界を跨ぐ設計
	•	新しい責務の追加
	•	契約（.proto）の変更
	•	エラー表現の追加

Copilotは「実装者」であり「設計者」ではありません。

⸻

9. このドキュメントの位置づけ
	•	本ファイルは 設計の一部
	•	CI / レビュー時の基準
	•	Copilot に対する「暗黙知の明文化」
