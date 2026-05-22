import { RegistrationForm } from '../features/auth/components/RegistrationForm.jsx'

export function RegistrationPage({ onRegister }) {
  return (
    <main className="registration-page">
      <RegistrationForm onRegister={onRegister} />
    </main>
  )
}
