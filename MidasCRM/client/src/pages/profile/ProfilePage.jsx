import { useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Profile.module.css'

const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')

export function ProfilePage({ user, onUpdateProfile }) {
  const [phoneNumber, setPhoneNumber] = useState(user?.phoneNumber ?? user?.phone ?? '')
  const [message, setMessage] = useState('')

  function submitProfile(event) {
    event.preventDefault()
    const normalizedPhone = phoneNumber.trim()

    onUpdateProfile({
      phoneNumber: normalizedPhone,
      phone: normalizedPhone,
    })
    setMessage('Телефон оновлено в особистому кабінеті.')
  }

  const displayName = [user?.name, user?.surname].filter(Boolean).join(' ') || user?.email || 'Користувач'

  return (
    <div className={cx('page-stack')}>
      <header className={cx('page-header')}>
        <div>
          <p className={cx('eyebrow')}>Profile</p>
          <h1>Особистий кабінет</h1>
        </div>
      </header>

      <section className={cx('panel', 'profile-grid')}>
        <div className={cx('profile-card')}>
          <div className={cx('profile-avatar')}>
            {displayName.slice(0, 1).toUpperCase()}
          </div>
          <div>
            <h2>{displayName}</h2>
            <p>{user?.email ?? '-'}</p>
          </div>
        </div>

        <form className={cx('profile-form')} onSubmit={submitProfile}>
          <Input label="Email" value={user?.email ?? ''} slotProps={{ input: { readOnly: true } }} />
          <Input label="Імʼя" value={user?.name ?? ''} slotProps={{ input: { readOnly: true } }} />
          <Input
            label="Мобільний телефон"
            placeholder="+380..."
            type="tel"
            value={phoneNumber}
            onChange={(event) => {
              setPhoneNumber(event.target.value)
              setMessage('')
            }}
          />
          {message && <div className={cx('profile-message')}>{message}</div>}
          <Button type="submit">Зберегти телефон</Button>
        </form>
      </section>
    </div>
  )
}
