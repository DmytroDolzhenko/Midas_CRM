import { RegistrationForm } from '../../features/auth/components/RegistrationForm.jsx'
import styles from '../../features/auth/styles/Auth.module.css'

export function RegistrationPage({ onRegister, theme = 'dark' }) {
  return (
    <main className={styles['auth-page']} data-theme={theme === 'dark' ? 'dark' : 'light'}>
      <RegistrationForm onRegister={onRegister} />
    </main>
  )
}

