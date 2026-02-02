import React from 'react';

function App() {
  return (
    <div className="app">
      <header>
        <h1>AI Text RPG Platform</h1>
        <p>AIと一緒に物語を作る体験</p>
      </header>
      <main>
        <section className="welcome">
          <h2>ようこそ</h2>
          <p>
            TextRpgは、AIとユーザーが共同で物語を進行・編集できる
            Webベースのテキストプラットフォームです。
          </p>
          <ul>
            <li>🧠 正史ノート（Canonical Notes）による記憶管理</li>
            <li>🎲 数値処理の完全分離</li>
            <li>🧩 プラグイン前提設計</li>
            <li>🖼️ 挿絵の自動生成</li>
          </ul>
        </section>
      </main>
    </div>
  );
}

export default App;
