import { useState } from 'react'

function extractValidationErrors(requestError) {
  if (requestError?.payload?.errors) {
    return requestError.payload.errors
  }

  if (!requestError?.message) {
    return null
  }

  try {
    const parsed = JSON.parse(requestError.message)

    if (Array.isArray(parsed)) {
      return { Password: parsed }
    }

    return parsed?.errors ?? null
  } catch {
    return null
  }
}

function mapPasswordRule(rule) {
  const code = rule?.code ?? ''

  if (code === 'PasswordRequiresNonAlphanumeric') {
    return 'Пароль має містити щонайменше один спеціальний символ'
  }

  if (code === 'PasswordRequiresLower') {
    return 'Пароль має містити щонайменше одну малу літеру (a-z)'
  }

  if (code === 'PasswordRequiresUpper') {
    return 'Пароль має містити щонайменше одну велику літеру (A-Z)'
  }

  const description = rule?.description
  return typeof description === 'string' ? description : null
}

function mapFieldErrors(errors) {
  if (!errors) {
    return null
  }

  const mapped = { ...errors }
  const rawPasswordErrors = errors?.Password
  const passwordMessage = rawPasswordErrors?.[0]

  if (Array.isArray(rawPasswordErrors) && typeof rawPasswordErrors[0] === 'object') {
    mapped.Password = rawPasswordErrors.map(mapPasswordRule).filter(Boolean)
    return mapped
  }

  if (passwordMessage && passwordMessage.toLowerCase().includes('at least 6 characters')) {
    mapped.Password = ['Пароль має містити щонайменше 6 символів']
  }

  return mapped
}

export function useRegistrationForm(onRegister) {
  const [name, setName] = useState('')
  const [surname, setSurname] = useState('')
  const [fathername, setFathername] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [error, setError] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  async function submit(event) {
    event.preventDefault()
    setError('')
    setFieldErrors({})

    if (!name || !surname || !fathername || !email || !phoneNumber || !password) {
      setError('Заповніть усі обовʼязкові поля')
      return
    }

    if (password !== confirmPassword) {
      setError('Паролі не співпадають')
      return
    }

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
      const validationErrors = mapFieldErrors(extractValidationErrors(requestError))

      if (validationErrors) {
        setFieldErrors(validationErrors)
        setError('Пароль має містити щонайменше 6 символів, включати великі та малі літери, а також спеціальні символи')
      } else {
        setError(requestError.message || 'Не вдалося зареєструватися')
      }
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
    fieldErrors,
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
