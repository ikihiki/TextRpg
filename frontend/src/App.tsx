import { FormEvent, useEffect, useMemo, useState } from 'react'

type OAuthProvider = {
  id: string
  displayName: string
}

type AuthProvidersResponse = {
  passwordLoginEnabled: boolean
  oauthProviders: OAuthProvider[]
}

type RawAuthProvidersResponse = {
  passwordLoginEnabled: boolean
  oauthProviders?: OAuthProvider[]
  oAuthProviders?: OAuthProvider[]
}

type AuthUser = {
  userId: string
  displayName: string
  email?: string
  provider: string
}

type AuthStateResponse = {
  authenticated: boolean
  user: AuthUser | null
}

type LoginFormState = {
  email: string
  password: string
}

const configuredApiBaseUrl = (import.meta.env.VITE_BFF_BASE_URL as string | undefined)?.replace(/\/$/, '')
const apiBaseUrl = configuredApiBaseUrl && configuredApiBaseUrl.length > 0 ? configuredApiBaseUrl : ''

function App() {
  const [providers, setProviders] = useState<AuthProvidersResponse | null>(null)
  const [authState, setAuthState] = useState<AuthStateResponse>({ authenticated: false, user: null })
  const [loginForm, setLoginForm] = useState<LoginFormState>({ email: '', password: '' })
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)

  const authStatusFromUrl = useMemo(() => {
    const params = new URLSearchParams(window.location.search)
    const auth = params.get('auth')
    const provider = params.get('provider')
    const authMessage = params.get('message')

    if (!auth) {
      return null
    }

    if (auth === 'success') {
      return `${provider ?? 'OAuth'} でログインしました。`
    }

    return authMessage ?? `${provider ?? 'OAuth'} ログインに失敗しました。`
  }, [])

  useEffect(() => {
    if (!authStatusFromUrl) {
      return
    }

    if (window.location.search.includes('auth=success')) {
      setMessage(authStatusFromUrl)
    } else {
      setError(authStatusFromUrl)
    }

    window.history.replaceState({}, document.title, window.location.pathname)
  }, [authStatusFromUrl])

  useEffect(() => {
    void loadAuthState()
  }, [])

  function normalizeProvidersResponse(response: RawAuthProvidersResponse): AuthProvidersResponse {
    return {
      passwordLoginEnabled: response.passwordLoginEnabled,
      oauthProviders: response.oauthProviders ?? response.oAuthProviders ?? [],
    }
  }

  async function loadAuthState() {
    setLoading(true)
    setError(null)

    try {
      const [providersResponse, meResponse] = await Promise.all([
        fetch(`${apiBaseUrl}/api/auth/providers`, { credentials: 'include' }),
        fetch(`${apiBaseUrl}/api/auth/me`, { credentials: 'include' }),
      ])

      if (!providersResponse.ok) {
        throw new Error('認証プロバイダ設定の取得に失敗しました。')
      }

      if (!meResponse.ok) {
        throw new Error('ログイン状態の取得に失敗しました。')
      }

      setProviders(normalizeProvidersResponse((await providersResponse.json()) as RawAuthProvidersResponse))
      setAuthState((await meResponse.json()) as AuthStateResponse)
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : '認証情報の読み込みに失敗しました。')
    } finally {
      setLoading(false)
    }
  }

  async function handlePasswordLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    setMessage(null)

    try {
      const response = await fetch(`${apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(loginForm),
      })

      if (!response.ok) {
        const body = (await response.json()) as { message?: string }
        throw new Error(body.message ?? 'ログインに失敗しました。')
      }

      setAuthState((await response.json()) as AuthStateResponse)
      setMessage('メール/パスワードでログインしました。')
      setLoginForm(current => ({ ...current, password: '' }))
    } catch (loginError) {
      setError(loginError instanceof Error ? loginError.message : 'ログインに失敗しました。')
    } finally {
      setSubmitting(false)
    }
  }

  async function handleLogout() {
    setSubmitting(true)
    setError(null)
    setMessage(null)

    try {
      const response = await fetch(`${apiBaseUrl}/api/auth/logout`, {
        method: 'POST',
        credentials: 'include',
      })

      if (!response.ok && response.status !== 204) {
        throw new Error('ログアウトに失敗しました。')
      }

      setAuthState({ authenticated: false, user: null })
      setMessage('ログアウトしました。')
    } catch (logoutError) {
      setError(logoutError instanceof Error ? logoutError.message : 'ログアウトに失敗しました。')
    } finally {
      setSubmitting(false)
    }
  }

  function beginOAuthLogin(provider: OAuthProvider) {
    const returnUrl = window.location.pathname || '/'
    window.location.href = `${apiBaseUrl}/api/auth/oauth/${encodeURIComponent(provider.id)}?returnUrl=${encodeURIComponent(returnUrl)}`
  }

  return (
    <main className="app-shell">
      <section className="hero-card">
        <p className="eyebrow">TextRpg Auth Foundation</p>
        <h1>AI Text RPG Platform</h1>
        <p className="description">
          AIとユーザーが共同で物語を進行・編集できるWebベースのテキストRPGプラットフォームです。
          まずは認証基盤として、メール/パスワードと OAuth ログインを利用できます。
        </p>
        <div className="status-row">
          <span className="status-label">BFF</span>
          <code>{apiBaseUrl}</code>
        </div>

        {message ? <p className="notice success">{message}</p> : null}
        {error ? <p className="notice error">{error}</p> : null}
      </section>

      <section className="auth-card">
        <header>
          <h2>ログイン</h2>
          <p>環境変数で有効化された認証方式だけ表示されます。</p>
        </header>

        {loading || !providers ? (
          <p>認証設定を読み込み中です...</p>
        ) : authState.authenticated && authState.user ? (
          <div className="signed-in-panel">
            <h3>ログイン中</h3>
            <dl>
              <div>
                <dt>表示名</dt>
                <dd>{authState.user.displayName}</dd>
              </div>
              <div>
                <dt>メール</dt>
                <dd>{authState.user.email ?? '未提供'}</dd>
              </div>
              <div>
                <dt>プロバイダ</dt>
                <dd>{authState.user.provider}</dd>
              </div>
              <div>
                <dt>ユーザーID</dt>
                <dd>{authState.user.userId}</dd>
              </div>
            </dl>

            <button type="button" onClick={handleLogout} disabled={submitting}>
              ログアウト
            </button>
          </div>
        ) : (
          <>
            {providers.passwordLoginEnabled ? (
              <form className="login-form" onSubmit={handlePasswordLogin}>
                <label>
                  Email
                  <input
                    autoComplete="email"
                    type="email"
                    value={loginForm.email}
                    onChange={event => setLoginForm(current => ({ ...current, email: event.target.value }))}
                    placeholder="user@example.com"
                    required
                  />
                </label>

                <label>
                  Password
                  <input
                    autoComplete="current-password"
                    type="password"
                    value={loginForm.password}
                    onChange={event => setLoginForm(current => ({ ...current, password: event.target.value }))}
                    placeholder="password"
                    required
                  />
                </label>

                <button type="submit" disabled={submitting}>
                  メール/パスワードでログイン
                </button>
              </form>
            ) : (
              <p className="helper-text">
                メール/パスワードログインは未設定です。AUTH_BOOTSTRAP_EMAIL と AUTH_BOOTSTRAP_PASSWORD を設定してください。
              </p>
            )}

            <div className="oauth-section">
              <h3>OAuth ログイン</h3>
              {providers.oauthProviders.length > 0 ? (
                <div className="oauth-actions">
                  {providers.oauthProviders.map(provider => (
                    <button
                      key={provider.id}
                      type="button"
                      className="secondary"
                      onClick={() => beginOAuthLogin(provider)}
                    >
                      {provider.displayName} でログイン
                    </button>
                  ))}
                </div>
              ) : (
                <p className="helper-text">
                  OAuth プロバイダは未設定です。AUTH_GOOGLE_* または AUTH_MICROSOFT_* を設定してください。
                </p>
              )}
            </div>
          </>
        )}
      </section>
    </main>
  )
}

export default App
