import { LoginForm } from '../../features/auth/components/LoginForm.jsx'
import styles from '../../features/auth/styles/Auth.module.css'

export function LoginPage({ onLogin, theme = 'dark' }) {
  return (
    <main className={styles['auth-page']} data-theme={theme === 'dark' ? 'dark' : 'light'}>
      <LoginForm onLogin={onLogin} />
    </main>
  )
}

