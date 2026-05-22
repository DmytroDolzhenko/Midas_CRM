import { LoginForm } from '../features/auth/components/LoginForm.jsx'

export function LoginPage({ onLogin }) {
  return (
    <main className="login-page">
      <LoginForm onLogin={onLogin} />
      <p className="login-hint">
        Ще немає акаунта? <a href="/register">Зареєструватися</a>
      </p>
    </main>
  )
}
