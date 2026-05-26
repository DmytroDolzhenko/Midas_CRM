import { useState } from 'react'

export function useRegistrationForm(onRegister) {
  const [name, setName] = useState('')
  const [surname, setSurname] = useState('')
  const [fathername, setFathername] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')

  async function submit(event) {
    event.preventDefault()

    if (!name || !surname || !fathername || !email || !phoneNumber || !password) {
      setError('Заповніть усі обовʼязкові поля')
      return
    }

    if (password !== confirmPassword) {
      setError('Паролі не співпадають')
      return
    }

    setError('')

    try {
      await onRegister({
        name,
        surname,
        fathername,
        phoneNumber,
        email,
        password,
      })
    } catch (requestError) {
      setError(requestError.message || 'Не вдалося зареєструватися')
    }
  }

  return {
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
  }
}
