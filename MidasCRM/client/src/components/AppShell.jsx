import { useState } from 'react'
import { integrations as defaultIntegrations } from '../lib/integrationsApi.js'

const navItems = [
  { id: 'dashboard', label: 'Головна' },
  { id: 'products', label: 'Товари' },
  { id: 'orders', label: 'Продажі' },
  { id: 'finances', label: 'Фінанси' },
  { id: 'customers', label: 'Клієнти' },
  { id: 'operations', label: 'Історія' },
]

function BellIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M18 16v-5a6 6 0 0 0-12 0v5l-2 2h16l-2-2Z" />
      <path d="M10 20a2 2 0 0 0 4 0" />
    </svg>
  )
}

function UserIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M20 21a8 8 0 0 0-16 0" />
      <circle cx="12" cy="8" r="4" />
    </svg>
  )
}

function SettingsIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M12 15.5a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Z" />
      <path d="M19.4 15a1.7 1.7 0 0 0 .34 1.88l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06A1.7 1.7 0 0 0 15 19.4a1.7 1.7 0 0 0-1 .6 1.7 1.7 0 0 0-.35 1v.17a2 2 0 1 1-4 0V21a1.7 1.7 0 0 0-.35-1 1.7 1.7 0 0 0-1-.6 1.7 1.7 0 0 0-1.88.34l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06A1.7 1.7 0 0 0 4.6 15a1.7 1.7 0 0 0-.6-1 1.7 1.7 0 0 0-1-.35H2.83a2 2 0 1 1 0-4H3a1.7 1.7 0 0 0 1-.35 1.7 1.7 0 0 0 .6-1 1.7 1.7 0 0 0-.34-1.88l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06A1.7 1.7 0 0 0 9 4.6a1.7 1.7 0 0 0 1-.6 1.7 1.7 0 0 0 .35-1V2.83a2 2 0 1 1 4 0V3a1.7 1.7 0 0 0 .35 1 1.7 1.7 0 0 0 1 .6 1.7 1.7 0 0 0 1.88-.34l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06A1.7 1.7 0 0 0 19.4 9c.2.37.4.7.6 1 .33.23.67.35 1 .35h.17a2 2 0 1 1 0 4H21a1.7 1.7 0 0 0-1 .35 1.7 1.7 0 0 0-.6 1Z" />
    </svg>
  )
}

export function AppShell({ activePage, user, theme, onThemeChange, onNavigate, onLogout, children }) {
  const [isAccountOpen, setIsAccountOpen] = useState(false)
  const [isSettingsOpen, setIsSettingsOpen] = useState(false)
  const [isNotificationsOpen, setIsNotificationsOpen] = useState(false)
  const [isAiOpen, setIsAiOpen] = useState(false)
  const [settingsTab, setSettingsTab] = useState('profile')
  const [integrations, setIntegrations] = useState(defaultIntegrations)
  const [settingsMessage, setSettingsMessage] = useState('')
  const activeItem = navItems.find((item) => item.id === activePage)

  function openSettings(tab = 'profile') {
    setIsAccountOpen(false)
    setSettingsTab(tab)
    setIsSettingsOpen(true)
  }

  function toggleIntegration(id) {
    setIntegrations((currentIntegrations) =>
      currentIntegrations.map((integration) =>
        integration.id === id ? { ...integration, enabled: !integration.enabled } : integration,
      ),
    )
    setSettingsMessage('Зміни інтеграцій підготовлено до збереження')
  }

  function saveSettings() {
    setSettingsMessage('Налаштування збережено')
    window.setTimeout(() => setIsSettingsOpen(false), 500)
  }

  return (
    <div className="app-shell" data-theme={theme}>
      <aside className="sidebar" aria-label="Головна навігація">
        <div className="brand">
          <span className="brand-mark"></span>
          <div>
            <strong>Midas CRM</strong>
            <span>Client workspace</span>
          </div>
        </div>

        <nav className="nav-list">
          {navItems.map((item) => (
            <button
              key={item.id}
              type="button"
              className={item.id === activePage ? 'nav-item active' : 'nav-item'}
              onClick={() => onNavigate(item.id)}
            >
              {item.label}
            </button>
          ))}
        </nav>

        <div className="sidebar-account">
          <button
            className="sidebar-account-button"
            type="button"
            aria-expanded={isAccountOpen}
            onClick={() => setIsAccountOpen((isOpen) => !isOpen)}
          >
            <span className="sidebar-account-icon">
              <UserIcon />
            </span>
            <span>
              <strong>{user?.name ?? 'Користувач'}</strong>
              <small>{user?.email}</small>
            </span>
          </button>

          {isAccountOpen && (
            <div className="sidebar-account-menu">
              <button type="button" onClick={() => openSettings('profile')}>
                <SettingsIcon />
                Налаштування
              </button>
              <button type="button" onClick={() => openSettings('integrations')}>
                Інтеграції
              </button>
              <button type="button" onClick={onLogout}>
                Вийти з акаунта
              </button>
            </div>
          )}
        </div>
      </aside>

      <div className="workspace">
        <header className="topbar">
          <div>
            <p className="topbar-label">Робочий простір</p>
            <h2>{activeItem?.label ?? 'Midas CRM'}</h2>
          </div>

          <div className="topbar-actions">
            <button className="secondary-button" type="button" onClick={() => onNavigate('createOrder')}>
              Додати продаж
            </button>
            <button className="secondary-button" type="button" onClick={() => onNavigate('finances')}>
              Додати витрату
            </button>
            <button className="secondary-button ai-button" type="button" onClick={() => setIsAiOpen(true)}>
              AI рекомендації
            </button>
            <button className="icon-button" type="button" aria-label="Налаштування" onClick={() => openSettings('profile')}>
              <SettingsIcon />
            </button>
            <button
              className="icon-button notification-button"
              type="button"
              aria-label="Сповіщення"
              onClick={() => setIsNotificationsOpen(true)}
            >
              <BellIcon />
              <span className="notification-dot" aria-hidden="true" />
            </button>
          </div>
        </header>

        <main className="main-surface">{children}</main>
      </div>

      {isSettingsOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsSettingsOpen(false)}>
          <section className="settings-modal" role="dialog" aria-modal="true" aria-labelledby="settings-title" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <h2 id="settings-title">Налаштування</h2>
              </div>
              <button className="modal-close-button" type="button" aria-label="Закрити налаштування" onClick={() => setIsSettingsOpen(false)}>
                ×
              </button>
            </div>

            <div className="settings-tabs">
              <button type="button" className={settingsTab === 'profile' ? 'tab-button active' : 'tab-button'} onClick={() => setSettingsTab('profile')}>
                Профіль
              </button>
              <button type="button" className={settingsTab === 'integrations' ? 'tab-button active' : 'tab-button'} onClick={() => setSettingsTab('integrations')}>
                Інтеграції
              </button>
            </div>

            {settingsTab === 'profile' && (
              <div className="settings-grid">
                <label className="field">
                  <span>Ім’я</span>
                  <input readOnly value={user?.name ?? ''} />
                </label>
                <label className="field">
                  <span>Email</span>
                  <input readOnly value={user?.email ?? ''} />
                </label>
                <label className="field">
                  <span>Мова інтерфейсу</span>
                  <select defaultValue="uk">
                    <option value="uk">Українська</option>
                    <option value="en">English</option>
                  </select>
                </label>
                <label className="field">
                  <span>Сповіщення</span>
                  <select defaultValue="important">
                    <option value="all">Усі події</option>
                    <option value="important">Тільки важливі</option>
                    <option value="off">Вимкнено</option>
                  </select>
                </label>
                <label className="field">
                  <span>Тема</span>
                  <select value={theme} onChange={(event) => onThemeChange(event.target.value)}>
                    <option value="light">Світла</option>
                    <option value="dark">Темна</option>
                  </select>
                </label>
              </div>
            )}

            {settingsTab === 'integrations' && (
              <div className="integration-grid">
                {integrations.map((integration) => (
                  <article className="integration-card" key={integration.id}>
                    <div>
                      <h3>{integration.name}</h3>
                      <p>{integration.description}</p>
                    </div>
                    <button className={integration.enabled ? 'toggle-button active' : 'toggle-button'} type="button" onClick={() => toggleIntegration(integration.id)}>
                      {integration.enabled ? 'Підключено' : 'Підключити'}
                    </button>
                  </article>
                ))}
              </div>
            )}

            {settingsMessage && <p className="settings-message">{settingsMessage}</p>}

            <div className="settings-actions">
              <button className="secondary-button" type="button" onClick={() => setIsSettingsOpen(false)}>
                Скасувати
              </button>
              <button className="primary-button" type="button" onClick={saveSettings}>
                Зберегти
              </button>
            </div>
          </section>
        </div>
      )}

      {isNotificationsOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsNotificationsOpen(false)}>
          <section className="notifications-modal" role="dialog" aria-modal="true" aria-labelledby="notifications-title" onClick={(event) => event.stopPropagation()}>
            <div className="notifications-header">
              <h2 id="notifications-title">Сповіщення</h2>
              <button className="modal-close-button" type="button" onClick={() => setIsNotificationsOpen(false)}>
                ×
              </button>
            </div>
            <div className="notifications-content">
              <p className="empty-state">Нових сповіщень немає</p>
            </div>
          </section>
        </div>
      )}

      {isAiOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsAiOpen(false)}>
          <section className="small-modal" role="dialog" aria-modal="true" aria-labelledby="ai-title" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">AI</p>
                <h2 id="ai-title">AI рекомендації</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setIsAiOpen(false)}>
                ×
              </button>
            </div>
            <div className="ai-recommendations">
              <p>
                Тут згодом AI аналізуватиме продажі, залишки, собівартість, маржу та витрати, щоб підказувати,
                які товари варто докупити, які краще не тримати в запасі, де збільшити вкладення, а де зменшити ризик.
              </p>
              <ul>
                <li>Рекомендувати товари з високою маржею та стабільними продажами.</li>
                <li>Попереджати про позиції, які довго лежать на складі.</li>
                <li>Показувати склади, де товару замало або забагато.</li>
                <li>Порівнювати витрати з прибутком і підсвічувати слабкі місця.</li>
              </ul>
            </div>
            <div className="settings-actions">
              <button className="primary-button" type="button" onClick={() => setIsAiOpen(false)}>
                Зрозуміло
              </button>
            </div>
          </section>
        </div>
      )}
    </div>
  )
}

