import { RegistrationForm } from '../../features/auth/components/RegistrationForm.jsx'

export function RegistrationPage({ onRegister }) {
  return (
    <main className="auth-page">
      <RegistrationForm onRegister={onRegister} />
    </main>
  )
}

