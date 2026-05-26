import { useState } from 'react'
import shared from '../../styles/Shared.module.css'
import styles from '../../styles/pages/Integrations.module.css'

function cx(...classes) {
  return classes.filter(Boolean).join(' ')
}

export function NovaPoshtaIntegrationPage({ connection, onBack, onConnect }) {
  const [token, setToken] = useState('')
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setSuccessMessage('')

    if (token.trim().length < 12) {
      setError('Вставте API token Нової Пошти. Мінімальна довжина для перевірки - 12 символів.')
      return
    }

    onConnect(token.trim())
    setSuccessMessage('Токен прийнято. Інтеграція Нової Пошти позначена як підключена.')
    setToken('')
  }

  return (
    <div className={shared['page-stack']}>
      <header className={shared['page-header']}>
        <div>
          <span className={styles.eyebrow}>Доставка</span>
          <h1>Нова Пошта</h1>
          <p className={styles.subtitle}>Підключіть API token, щоб далі додати перевірку ТТН, статус отримання посилок і автоматичні сповіщення.</p>
        </div>
        <button className={shared['secondary-button']} type="button" onClick={onBack}>Назад</button>
      </header>

      <section className={cx(shared.panel, styles['integration-layout'])}>
        <div className={styles['instruction-list']}>
          <h2>Як підключити</h2>
          <ol>
            <li>Відкрийте бізнес-кабінет Нової Пошти.</li>
            <li>Створіть або скопіюйте API token у розділі налаштувань.</li>
            <li>Вставте token у форму праворуч і натисніть кнопку підключення.</li>
          </ol>
          <div className={styles['status-card']}>
            <span>Статус</span>
            <strong>{connection?.connected ? 'Підключено' : 'Очікує token'}</strong>
            {connection?.tokenPreview && <small>Token: {connection.tokenPreview}</small>}
          </div>
        </div>

        <form className={styles['token-form']} onSubmit={handleSubmit}>
          <label className={shared.field}>
            <span>API token</span>
            <input
              autoComplete="off"
              placeholder="Вставте token Нової Пошти"
              type="password"
              value={token}
              onChange={(event) => setToken(event.target.value)}
            />
          </label>
          {error && <div className="form-error">{error}</div>}
          {successMessage && <div className={styles['success-message']}>{successMessage}</div>}
          <button className={shared['primary-button']} type="submit">
            Перевірити та підключити
          </button>
        </form>
      </section>
    </div>
  )
}
