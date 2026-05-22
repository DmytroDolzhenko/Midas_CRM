import { useRegistrationForm } from '../hooks/useRegistrationForm.js'
import { Button } from '../../../components/Button.jsx'
import { Input } from '../../../components/Input.jsx'


export function RegistrationForm({ onRegister }) {
  const { email, password, confirmPassword, error, setEmail, setPassword, setConfirmPassword, submit } = useRegistrationForm(onRegister)

  return (
    <form className="login-card" onSubmit={submit}>
      <div>
        <p className="eyebrow">Midas CRM</p>
        <h1>Реєстрація</h1>
      </div>

      <Input
        label="Email"
        type="email"
        value={email}
        onChange={(event) => setEmail(event.target.value)}
      />
      <Input
        label="Пароль"
        type="password"
        value={password}
        onChange={(event) => setPassword(event.target.value)}
          />
       <Input
        label="Підтвердження пароля"
        type="password"
        value={confirmPassword}
        onChange={(event) => setConfirmPassword(event.target.value)}
      />

      {error && <p className="form-error">{error}</p>}

      <Button className="full-width" type="submit">
        Зареєструватися
      </Button>
    </form>
  )
}
