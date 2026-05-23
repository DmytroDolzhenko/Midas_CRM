import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Company.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')

const OWNER_ROLE = 1
const ADMIN_ROLE = 2

const roleOptions = [
  { value: 1, label: 'Власник' },
  { value: 2, label: 'Адміністратор' },
  { value: 3, label: 'Менеджер' },
  { value: 4, label: 'Складальник' },
]

const roleLabels = new Map(roleOptions.map((role) => [role.value, role.label]))

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function getMemberName(member) {
  const user = getValue(member, 'user', 'User')
  if (user) {
    const name = getValue(user, 'name', 'Name') ?? ''
    const surname = getValue(user, 'surname', 'Surname') ?? ''
    const fullName = `${name} ${surname}`.trim()
    return fullName || getValue(user, 'email', 'Email') || 'Користувач'
  }

  return String(getValue(member, 'userId', 'UserId'))
}

function getMemberKey(member) {
  return String(getValue(member, 'id', 'Id') ?? getValue(member, 'userId', 'UserId'))
}

export function CompanyPage({
  activeCompany,
  currentUserId,
  onCreateCompany,
  onUpdateCompany,
  onDeleteCompany,
  onAddMember,
  onChangeMemberRole,
  onRemoveMember,
  isBusy,
  error,
}) {
  const [createName, setCreateName] = useState('')
  const [createTaxNumber, setCreateTaxNumber] = useState('')
  const [editName, setEditName] = useState(activeCompany?.name ?? activeCompany?.Name ?? '')
  const [editTaxNumber, setEditTaxNumber] = useState(activeCompany?.taxNumber ?? activeCompany?.TaxNumber ?? '')
  const [memberEmail, setMemberEmail] = useState('')

  const members = useMemo(() => getValue(activeCompany, 'members', 'Members') ?? [], [activeCompany])

  const currentMember = members.find((member) => String(getValue(member, 'userId', 'UserId')) === String(currentUserId))
  const currentRole = Number(getValue(currentMember, 'role', 'Role') ?? 0)
  const canManageCompany = currentRole === OWNER_ROLE || currentRole === ADMIN_ROLE

  function submitCreate(event) {
    event.preventDefault()
    onCreateCompany({
      name: createName.trim(),
      taxNumber: createTaxNumber.trim() || null,
    }).then(() => {
      setCreateName('')
      setCreateTaxNumber('')
    })
  }

  function submitUpdate(event) {
    event.preventDefault()
    onUpdateCompany({
      name: editName.trim(),
      taxNumber: editTaxNumber.trim() || null,
    })
  }

  function submitAddMember(event) {
    event.preventDefault()
    onAddMember(memberEmail.trim()).then(() => setMemberEmail(''))
  }

  return (
    <div className={cx('page-stack')}>
      <section className={cx('page-header')}>
        <div>
          <p className={cx('eyebrow')}>Company</p>
          <h1>Керування компанією</h1>
        </div>
      </section>

      {error && <div className={cx('api-error-banner')}><span>{error}</span></div>}

      <section className={cx('panel')}>
        <h2>Створити нову компанію</h2>
        <form className={cx('form-grid')} onSubmit={submitCreate}>
          <div className={cx('form-section')}>
            <Input label="Назва" value={createName} onChange={(event) => setCreateName(event.target.value)} required />
            <Input label="Податковий номер" value={createTaxNumber} onChange={(event) => setCreateTaxNumber(event.target.value)} />
          </div>
          <div className={cx('summary-panel')}>
            <Button type="submit" disabled={isBusy || !createName.trim()}>Створити компанію</Button>
          </div>
        </form>
      </section>

      <section className={cx('panel')}>
        <h2>Поточна компанія</h2>
        <form className={cx('form-grid')} onSubmit={submitUpdate}>
          <div className={cx('form-section')}>
            <Input label="Назва" value={editName} onChange={(event) => setEditName(event.target.value)} disabled={!canManageCompany} required />
            <Input label="Податковий номер" value={editTaxNumber ?? ''} onChange={(event) => setEditTaxNumber(event.target.value)} disabled={!canManageCompany} />
          </div>
          <div className={cx('summary-panel', 'company-actions')}>
            <Button type="submit" disabled={!canManageCompany || isBusy || !editName.trim()}>Зберегти зміни</Button>
            <Button type="button" variant="secondary" disabled={!canManageCompany || isBusy} onClick={onDeleteCompany}>Видалити компанію</Button>
          </div>
        </form>
      </section>

      <section className={cx('panel')}>
        <h2>Учасники компанії</h2>

        {canManageCompany && (
          <form className={cx('toolbar')} onSubmit={submitAddMember}>
            <input
              type="email"
              placeholder="email користувача"
              value={memberEmail}
              onChange={(event) => setMemberEmail(event.target.value)}
              required
            />
            <Button type="submit" disabled={isBusy}>Додати</Button>
          </form>
        )}

        <div className={cx('table-header', 'company-members-table')}>
          <span>Користувач</span>
          <span>Email</span>
          <span>Роль</span>
          <span>Дії</span>
        </div>

        {!members.length && (
          <div className={cx('member-empty')}>
            У цій компанії ще немає учасників.
          </div>
        )}

        {members.map((member) => {
          const memberUser = getValue(member, 'user', 'User')
          const memberUserId = String(getValue(member, 'userId', 'UserId'))
          const memberRole = Number(getValue(member, 'role', 'Role'))
          const isCurrentUser = memberUserId === String(currentUserId)
          const canModifyMember = canManageCompany && memberRole !== OWNER_ROLE

          return (
            <div className={cx('table-row', 'company-members-table')} key={getMemberKey(member)}>
              <span className={cx('member-primary')}>{getMemberName(member)}</span>
              <span className={cx('member-muted')}>{getValue(memberUser, 'email', 'Email') ?? '-'}</span>
              <span>
                {canModifyMember ? (
                  <select
                    className={cx('member-role-select')}
                    value={memberRole}
                    onChange={(event) => onChangeMemberRole(memberUserId, Number(event.target.value))}
                    disabled={isBusy}
                  >
                    {roleOptions.map((role) => (
                      <option value={role.value} key={role.value}>{role.label}</option>
                    ))}
                  </select>
                ) : (
                  <span className={cx('member-role-label')}>{roleLabels.get(memberRole) ?? memberRole}</span>
                )}
              </span>
              <span className={cx('member-actions')}>
                {canModifyMember && !isCurrentUser ? (
                  <Button type="button" variant="secondary" disabled={isBusy} onClick={() => onRemoveMember(memberUserId)}>
                    Видалити
                  </Button>
                ) : (
                  '-'
                )}
              </span>
            </div>
          )
        })}
      </section>
    </div>
  )
}
