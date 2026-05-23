import { useRegistrationForm } from '../hooks/useRegistrationForm.js'
import { Button } from '../../../components/Button.jsx'
import { Input } from '../../../components/Input.jsx'
import styles from '../styles/Auth.module.css'

export function RegistrationForm({ onRegister }) {
  const {
    name,
    surname,
    fathername,
    phoneNumber,
    email,
    password,
    confirmPassword,
    error,
    setName,
    setSurname,
    setFathername,
    setPhoneNumber,
    setEmail,
    setPassword,
    setConfirmPassword,
    submit,
  } = useRegistrationForm(onRegister)

  return (
    <form className={`${styles['auth-card']} ${styles['auth-card-wide']}`} onSubmit={submit}>
      <div className={styles['auth-card-header']}>
        <p className="eyebrow">Midas CRM</p>
        <h1>Створення акаунта</h1>
        <p>Заповніть дані користувача. Після реєстрації система автоматично виконає вхід.</p>
      </div>

      <div className={styles['auth-switch']}>
        <a href="/login">Увійти</a>
        <a className={styles.active} href="/register">Зареєструватись</a>
      </div>

      <div className={styles['auth-form-grid']}>
        <Input label="Імʼя" value={name} onChange={(event) => setName(event.target.value)} />
        <Input label="Прізвище" value={surname} onChange={(event) => setSurname(event.target.value)} />
        <Input label="По батькові" value={fathername} onChange={(event) => setFathername(event.target.value)} />
        <Input label="Телефон" value={phoneNumber} onChange={(event) => setPhoneNumber(event.target.value)} />
        <Input label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} />
        <Input label="Пароль" type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
        <Input
          className={styles['auth-grid-span']}
          label="Підтвердження пароля"
          type="password"
          value={confirmPassword}
          onChange={(event) => setConfirmPassword(event.target.value)}
        />
      </div>

      {error && <p className="form-error">{error}</p>}

      <Button className="full-width" type="submit">
        Зареєструватись
      </Button>
    </form>
  )
}
