import { useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'

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
    <main className="company-setup-page">
      <section className="company-setup-card">
        <div className="company-setup-copy">
          <p className="eyebrow">Перший запуск</p>
          <h1>Створіть компанію</h1>
          <p>
            Компанія обʼєднує товари, продажі, клієнтів і фінанси в одному робочому просторі.
            Після створення ви одразу перейдете до CRM.
          </p>
          <div className="company-user-chip">{userEmail}</div>
        </div>

        <form className="company-setup-form" onSubmit={handleSubmit}>
          <Input
            label="Назва компанії"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="Наприклад, Gachi Store"
            required
          />

          <Input
            label="Податковий номер"
            value={taxNumber}
            onChange={(event) => setTaxNumber(event.target.value)}
            placeholder="Необовʼязково"
          />

          {(formError || error) && <p className="form-error">{formError || error}</p>}

          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створюємо...' : 'Створити компанію'}
          </Button>

          <Button className="full-width" type="button" variant="secondary" onClick={onLogout}>
            Вийти
          </Button>
        </form>
      </section>
    </main>
  )
}

