import { useState } from 'react'
import { serverApi } from '../../lib/serverApi.js'
import shared from '../../styles/Shared.module.css'
import styles from '../../styles/pages/Integrations.module.css'

function cx(...classes) {
  return classes.filter(Boolean).join(' ')
}

export function NovaPoshtaLogisticProfilePage({ onBack, onSaved }) {
  const [sendersPhone, setSendersPhone] = useState('')
  const [cityName, setCityName] = useState('')
  const [warehouseQuery, setWarehouseQuery] = useState('')
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setSuccessMessage('')

    if (!sendersPhone.trim() || !cityName.trim() || !warehouseQuery.trim()) {
      setError('Заповніть номер телефону, місто та відділення.')
      return
    }

    setIsSubmitting(true)

    try {
      await serverApi.novaPoshta.saveLogisticProfile({
        sendersPhone: sendersPhone.trim(),
        cityName: cityName.trim(),
        warehouseQuery: warehouseQuery.trim(),
      })
      setSuccessMessage('Профіль логістики успішно збережено.')
      onSaved?.()
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося зберегти логістичний профіль.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className={shared['page-stack']}>
      <header className={shared['page-header']}>
        <div>
          <span className={styles.eyebrow}>Логістика</span>
          <h1>Профіль Нової Пошти</h1>
          <p className={styles.subtitle}>Вкажіть дані відправника для автоматичного створення ТТН.</p>
        </div>
        <button className={shared['secondary-button']} type="button" onClick={onBack}>Назад</button>
      </header>

      <section className={cx(shared.panel, styles['integration-layout'])}>
        <div className={styles['instruction-list']}>
          <h2>Що потрібно</h2>
          <ol>
            <li>Телефон відправника у форматі +380...</li>
            <li>Назва міста відправника (як у Новій Пошті).</li>
            <li>Номер або назва відділення/поштомату.</li>
          </ol>
        </div>

        <form className={styles['token-form']} onSubmit={handleSubmit}>
          <label className={shared.field}>
            <span>Телефон відправника</span>
            <input value={sendersPhone} onChange={(event) => setSendersPhone(event.target.value)} placeholder="+380..." />
          </label>
          <label className={shared.field}>
            <span>Місто</span>
            <input value={cityName} onChange={(event) => setCityName(event.target.value)} placeholder="Київ" />
          </label>
          <label className={shared.field}>
            <span>Відділення або поштомат</span>
            <input value={warehouseQuery} onChange={(event) => setWarehouseQuery(event.target.value)} placeholder="1 або Відділення №1" />
          </label>

          {error && <div className="form-error">{error}</div>}
          {successMessage && <div className={styles['success-message']}>{successMessage}</div>}

          <button className={shared['primary-button']} type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Збереження...' : 'Зберегти профіль'}
          </button>
        </form>
      </section>
    </div>
  )
}
