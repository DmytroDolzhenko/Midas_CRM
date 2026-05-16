import { Button } from '../../../components/Button.jsx'
import { Input } from '../../../components/Input.jsx'
import { useLoginForm } from '../hooks/useLoginForm.js'

export function LoginForm({ onLogin }) {
  const { email, password, error, setEmail, setPassword, submit } = useLoginForm(onLogin)

  return (
    <form className="login-card" onSubmit={submit}>
      <div>
        <p className="eyebrow">Midas CRM</p>
        <h1>Вхід у систему</h1>
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

      {error && <p className="form-error">{error}</p>}

      <Button className="full-width" type="submit">
        Увійти
      </Button>
    </form>
  )
}
