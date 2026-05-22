import { useState } from 'react'

export function useLoginForm(onLogin) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  async function submit(event) {
    event.preventDefault()

    if (!email || !password) {
      setError('Введіть email і пароль')
      return
    }

    setError('')

    try {
      await onLogin({ email, password })
    } catch (requestError) {
      setError(requestError.message || 'Не вдалося увійти в систему')
    }
  }

  return {
    email,
    password,
    error,
    setEmail,
    setPassword,
    submit,
  }
}
