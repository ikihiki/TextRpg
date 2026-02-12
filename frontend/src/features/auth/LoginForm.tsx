import { useState } from 'react'

interface LoginFormProps {
  onLogin: (email: string, password: string) => void
  onSwitchToRegister: () => void
}

export function LoginForm({ onLogin, onSwitchToRegister }: LoginFormProps) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    
    if (!email || !password) {
      setError('メールアドレスとパスワードを入力してください')
      return
    }

    onLogin(email, password)
  }

  return (
    <div className="auth-form">
      <h2>ログイン</h2>
      <form onSubmit={handleSubmit}>
        {error && <div className="error-message">{error}</div>}
        <div className="form-group">
          <label htmlFor="email">メールアドレス</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="email@example.com"
          />
        </div>
        <div className="form-group">
          <label htmlFor="password">パスワード</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="パスワード"
          />
        </div>
        <button type="submit" className="primary-button">
          ログイン
        </button>
      </form>
      <p className="auth-switch">
        アカウントをお持ちでない方は{' '}
        <button type="button" onClick={onSwitchToRegister} className="link-button">
          新規登録
        </button>
      </p>
    </div>
  )
}
