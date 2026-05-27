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
    fieldErrors,
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
        {/* 2. Додаємо проп error у компоненти інпутів. 
            Сервер повертає назви з великої літери (Password, Email), тому пишемо fieldErrors.Password */}
        <Input 
          label="Імʼя" 
          value={name} 
          onChange={(event) => setName(event.target.value)} 
          error={fieldErrors?.Name?.[0]} 
        />
        <Input 
          label="Прізвище" 
          value={surname} 
          onChange={(event) => setSurname(event.target.value)} 
          error={fieldErrors?.Surname?.[0]} 
        />
        <Input 
          label="По батькові" 
          value={fathername} 
          onChange={(event) => setFathername(event.target.value)} 
          error={fieldErrors?.Fathername?.[0]} 
        />
        <Input 
          label="Телефон" 
          value={phoneNumber} 
          onChange={(event) => setPhoneNumber(event.target.value)} 
          error={fieldErrors?.PhoneNumber?.[0]} 
        />
        <Input 
          label="Email" 
          type="email" 
          value={email} 
          onChange={(event) => setEmail(event.target.value)} 
          error={fieldErrors?.Email?.[0]} 
        />
        <Input 
          label="Пароль" 
          type="password" 
          value={password} 
          onChange={(event) => setPassword(event.target.value)} 
          error={fieldErrors?.Password?.[0]}
        />
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
