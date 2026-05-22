import { LoginForm } from '../../features/auth/components/LoginForm.jsx'

export function LoginPage({ onLogin }) {
  return (
    <main className="auth-page">
      <LoginForm onLogin={onLogin} />
    </main>
  )
}

