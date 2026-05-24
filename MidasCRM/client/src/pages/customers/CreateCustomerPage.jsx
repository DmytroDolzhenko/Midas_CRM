import { useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'
import sharedStyles from '../../styles/Shared.module.css'

const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')

export function CreateCustomerPage({ onBack, onCreate }) {
  const [name, setName] = useState('')
  const [surname, setSurname] = useState('')
  const [contactValue, setContactValue] = useState('')
  const [email, setEmail] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await onCreate({
        name: name.trim(),
        surname: surname.trim(),
        contactValue: contactValue.trim(),
        email: email.trim(),
      })
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося створити клієнта')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className={cx('page-stack')}>
      <div className={cx('page-header')}>
        <div>
          <p className="eyebrow">Customers</p>
          <h1>Новий клієнт</h1>
        </div>
        <button className={cx('secondary-button')} type="button" onClick={onBack}>
          Назад
        </button>
      </div>

      <form className={cx('wide-form')} onSubmit={handleSubmit}>
        <section className={cx('panel', 'form-section')}>
          <div className={cx('form-grid-3')}>
            <Input label="Імʼя" value={name} onChange={(event) => setName(event.target.value)} required />
            <Input label="Прізвище" value={surname} onChange={(event) => setSurname(event.target.value)} required />
            <Input label="Телефон" value={contactValue} onChange={(event) => setContactValue(event.target.value)} required />
            <Input className={cx('span-2')} label="Email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} required />
          </div>
          {error && <p className="form-error">{error}</p>}
        </section>

        <aside className={cx('panel', 'summary-panel')}>
          <h2>Дані для сервера</h2>
          <div className={cx('summary-line')}>
            <span>Name</span>
            <strong>{name || '-'}</strong>
          </div>
          <div className={cx('summary-line')}>
            <span>Surname</span>
            <strong>{surname || '-'}</strong>
          </div>
          <div className={cx('summary-line')}>
            <span>ContactValue</span>
            <strong>{contactValue || '-'}</strong>
          </div>
          <Button className="full-width" type="submit" disabled={isSubmitting || !name.trim() || !surname.trim() || !contactValue.trim() || !email.trim()}>
            Створити клієнта
          </Button>
        </aside>
      </form>
    </section>
  )
}
