import { useState } from 'react'

interface RegisterFormProps {
  onRegister: (email: string, password: string) => void
  onSwitchToLogin: () => void
}

export function RegisterForm({ onRegister, onSwitchToLogin }: RegisterFormProps) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setError('')

    if (!email || !password || !confirmPassword) {
      setError('すべての項目を入力してください')
      return
    }

    if (password !== confirmPassword) {
      setError('パスワードが一致しません')
      return
    }

    if (password.length < 8) {
      setError('パスワードは8文字以上で入力してください')
      return
    }

    if (!/[A-Z]/.test(password) || !/[a-z]/.test(password) || !/[0-9]/.test(password)) {
      setError('パスワードは大文字、小文字、数字を含む必要があります')
      return
    }

    onRegister(email, password)
  }

  return (
    <div className="auth-form">
      <h2>新規登録</h2>
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
            placeholder="8文字以上、大小文字・数字を含む"
          />
        </div>
        <div className="form-group">
          <label htmlFor="confirmPassword">パスワード（確認）</label>
          <input
            id="confirmPassword"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            placeholder="パスワードを再入力"
          />
        </div>
        <button type="submit" className="primary-button">
          登録
        </button>
      </form>
      <p className="auth-switch">
        既にアカウントをお持ちの方は{' '}
        <button type="button" onClick={onSwitchToLogin} className="link-button">
          ログイン
        </button>
      </p>
    </div>
  )
}
