import { useState } from 'react'
import { LoginForm, RegisterForm, Profile } from './features/auth'

type AuthState = 'login' | 'register' | 'authenticated'

interface UserProfile {
  userId: string
  email: string
  displayName: string
  iconUrl?: string
  bio?: string
  language: string
}

function App() {
  const [authState, setAuthState] = useState<AuthState>('login')
  const [user, setUser] = useState<UserProfile | null>(null)
  const [error, setError] = useState<string>('')

  // Mock handlers - in production, these would call the BFF gRPC service
  const handleLogin = (email: string, _password: string) => {
    // TODO: Call BFF UserApi.Login via gRPC-Web
    console.log('Login attempt:', email)
    
    // Mock successful login
    setUser({
      userId: 'mock-user-id',
      email: email,
      displayName: email.split('@')[0],
      language: 'ja'
    })
    setAuthState('authenticated')
    setError('')
  }

  const handleRegister = (email: string, _password: string) => {
    // TODO: Call BFF UserApi.Register via gRPC-Web
    console.log('Register attempt:', email)
    
    // Mock successful registration
    setUser({
      userId: 'mock-user-id',
      email: email,
      displayName: email.split('@')[0],
      language: 'ja'
    })
    setAuthState('authenticated')
    setError('')
  }

  const handleUpdateProfile = (displayName: string, bio: string, language: string) => {
    // TODO: Call BFF UserApi.UpdateProfile via gRPC-Web
    console.log('Update profile:', { displayName, bio, language })
    
    if (user) {
      setUser({
        ...user,
        displayName,
        bio,
        language
      })
    }
  }

  const handleLogout = () => {
    // TODO: Call BFF UserApi.Logout via gRPC-Web
    console.log('Logout')
    
    setUser(null)
    setAuthState('login')
  }

  const handleDeleteAccount = () => {
    // TODO: Call BFF UserApi.DeleteAccount via gRPC-Web
    console.log('Delete account')
    
    setUser(null)
    setAuthState('login')
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>AI Text RPG Platform</h1>
        <p>AIとユーザーが共同で物語を進行・編集できるWebベースのテキストRPGプラットフォームです。</p>
      </header>

      <main className="app-main">
        {error && <div className="error-banner">{error}</div>}
        
        {authState === 'login' && (
          <LoginForm
            onLogin={handleLogin}
            onSwitchToRegister={() => setAuthState('register')}
          />
        )}
        
        {authState === 'register' && (
          <RegisterForm
            onRegister={handleRegister}
            onSwitchToLogin={() => setAuthState('login')}
          />
        )}
        
        {authState === 'authenticated' && user && (
          <Profile
            userId={user.userId}
            email={user.email}
            displayName={user.displayName}
            iconUrl={user.iconUrl}
            bio={user.bio}
            language={user.language}
            onUpdateProfile={handleUpdateProfile}
            onLogout={handleLogout}
            onDeleteAccount={handleDeleteAccount}
          />
        )}
      </main>
    </div>
  )
}

export default App
