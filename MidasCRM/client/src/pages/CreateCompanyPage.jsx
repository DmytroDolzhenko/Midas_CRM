import { useState } from 'react'
import { Button } from '../components/Button.jsx'
import { Input } from '../components/Input.jsx'

export function CreateCompanyPage({ userEmail, onCreateCompany, isSubmitting, error, onLogout }) {
  const [name, setName] = useState('')
  const [taxNumber, setTaxNumber] = useState('')
  const [formError, setFormError] = useState('')

  async function handleSubmit(event) {
    event.preventDefault()

    if (!name.trim()) {
      setFormError('Вкажіть назву компанії')
      return
    }

    setFormError('')

    await onCreateCompany({
      name: name.trim(),
      taxNumber: taxNumber.trim() || null,
    })
  }

  return (
    <main className="login-page">
      <form className="login-card" onSubmit={handleSubmit}>
        <div>
          <p className="eyebrow">Midas CRM</p>
          <h1>Створити компанію</h1>
          <p className="create-company-subtitle">
            {userEmail} не є учасником жодної компанії. Створіть компанію, щоб продовжити роботу.
          </p>
        </div>

        <Input
          label="Назва компанії"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="ТОВ Мідас"
          required
        />

        <Input
          label="Податковий номер (необов'язково)"
          value={taxNumber}
          onChange={(event) => setTaxNumber(event.target.value)}
          placeholder="12345678"
        />

        {(formError || error) && <p className="form-error">{formError || error}</p>}

        <Button className="full-width" type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'Створення...' : 'Створити компанію'}
        </Button>

        <Button className="full-width" type="button" variant="secondary" onClick={onLogout}>
          Вийти
        </Button>
      </form>
    </main>
  )
}