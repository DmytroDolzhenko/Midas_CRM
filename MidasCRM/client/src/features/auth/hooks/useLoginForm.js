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
    await onLogin({ email, password })
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
