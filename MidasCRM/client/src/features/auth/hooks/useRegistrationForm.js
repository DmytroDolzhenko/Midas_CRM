import { useState } from 'react'

export function useRegistrationForm(onRegister) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')

  async function submit(event) {
    event.preventDefault()

    if (!email || !password) {
      setError('Введи email і пароль')
      return
    }

    setError('')

    try {
      await onRegister({ email, password })
    } catch (error) {
      setError(error.message || 'Не вдалося зареєструватися')
    }

    if (password !== confirmPassword) {
      setError('Паролі не співпадають')
      return
    }

  }

  return {
    email,
    password,
    confirmPassword,
    error,
    setEmail,
    setPassword,
    setConfirmPassword,
    submit,
  }
}
