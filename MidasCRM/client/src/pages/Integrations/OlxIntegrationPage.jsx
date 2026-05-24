import { useState } from 'react'
import shared from '../../styles/Shared.module.css'
import styles from '../../styles/pages/Integrations.module.css'

function cx(...classes) {
  return classes.filter(Boolean).join(' ')
}

export function OlxIntegrationPage({ connection, onBack, onConnect }) {
  const [token, setToken] = useState('')
  const [error, setError] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setSuccessMessage('')

    if (token.trim().length < 12) {
      setError('Вставте access token OLX. Мінімальна довжина для перевірки - 12 символів.')
      return
    }

    onConnect(token.trim())
    setSuccessMessage('Токен прийнято. Інтеграція OLX позначена як підключена.')
    setToken('')
  }

  return (
    <div className={shared['page-stack']}>
      <header className={shared['page-header']}>
        <div>
          <span className={styles.eyebrow}>Маркетплейс</span>
          <h1>OLX</h1>
          <p className={styles.subtitle}>Підготуйте підключення OLX для майбутньої синхронізації оголошень, заявок покупців і замовлень.</p>
        </div>
        <button className={shared['secondary-button']} type="button" onClick={onBack}>Назад</button>
      </header>

      <section className={cx(shared.panel, styles['integration-layout'])}>
        <div className={styles['instruction-list']}>
          <h2>Як підключити</h2>
          <ol>
            <li>Підготуйте access token або OAuth ключі OLX.</li>
            <li>Перевірте, що акаунт має доступ до оголошень і повідомлень.</li>
            <li>Вставте token у форму праворуч, щоб зберегти статус підключення.</li>
          </ol>
          <div className={styles['status-card']}>
            <span>Статус</span>
            <strong>{connection?.connected ? 'Підключено' : 'Очікує token'}</strong>
            {connection?.tokenPreview && <small>Token: {connection.tokenPreview}</small>}
          </div>
        </div>

        <form className={styles['token-form']} onSubmit={handleSubmit}>
          <label className={shared.field}>
            <span>Access token</span>
            <input
              autoComplete="off"
              placeholder="Вставте token OLX"
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
