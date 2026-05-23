import { Button } from '../../../components/Button.jsx'
import { Input } from '../../../components/Input.jsx'
import { useLoginForm } from '../hooks/useLoginForm.js'
import styles from '../styles/Auth.module.css'

export function LoginForm({ onLogin }) {
  const { email, password, error, setEmail, setPassword, submit } = useLoginForm(onLogin)

  return (
    <form className={styles['auth-card']} onSubmit={submit}>
      <div className={styles['auth-card-header']}>
        <p className="eyebrow">Midas CRM</p>
        <h1>Вхід у систему</h1>
        <p>Увійдіть у робочий простір, щоб керувати продажами, товарами та фінансами.</p>
      </div>

      <div className={styles['auth-switch']}>
        <a className={styles.active} href="/login">Увійти</a>
        <a href="/register">Зареєструватись</a>
      </div>

      <Input label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} />
      <Input label="Пароль" type="password" value={password} onChange={(event) => setPassword(event.target.value)} />

      {error && <p className="form-error">{error}</p>}

      <Button className="full-width" type="submit">
        Увійти
      </Button>
    </form>
  )
}
