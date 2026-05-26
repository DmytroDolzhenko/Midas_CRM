import { useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Company.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



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
    <main className={cx('company-setup-page')}>
      <section className={cx('company-setup-card')}>
        <div className={cx('company-setup-copy')}>
          <p className={cx('eyebrow')}>Перший запуск</p>
          <h1>Створіть компанію</h1>
          <p>
            Компанія обʼєднує товари, продажі, клієнтів і фінанси в одному робочому просторі.
            Після створення ви одразу перейдете до CRM.
          </p>
          <div className={cx('company-user-chip')}>{userEmail}</div>
        </div>

        <form className={cx('company-setup-form')} onSubmit={handleSubmit}>
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

          {(formError || error) && <p className={cx('form-error')}>{formError || error}</p>}

          <Button className={cx('full-width')} type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створюємо...' : 'Створити компанію'}
          </Button>

          <Button className={cx('full-width')} type="button" variant="secondary" onClick={onLogout}>
            Вийти
          </Button>
        </form>
      </section>
    </main>
  )
}

