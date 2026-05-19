import { useState } from 'react'

export function useLoginForm(onLogin) {
  const [email, setEmail] = useState('admin@midas.test')
  const [password, setPassword] = useState('password')
  const [error, setError] = useState('')

  async function submit(event) {
    event.preventDefault()

    if (!email || !password) {
      setError('Введи email і пароль')
      return
    }

    setError('')

    try {
      await onLogin({ email, password })
    } catch (error) {
      setError(error.message || 'Не вдалося увійти в систему')
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
