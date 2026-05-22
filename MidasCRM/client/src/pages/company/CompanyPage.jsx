import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'

const OWNER_ROLE = 1
const ADMIN_ROLE = 2

const roleOptions = [
  { value: 1, label: 'Р’Р»Р°СЃРЅРёРє' },
  { value: 2, label: 'РђРґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂ' },
  { value: 3, label: 'РњРµРЅРµРґР¶РµСЂ' },
  { value: 4, label: 'РЎРєР»Р°РґР°Р»СЊРЅРёРє' },
]

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function getMemberName(member) {
  const user = getValue(member, 'user', 'User')
  if (user) {
    const name = getValue(user, 'name', 'Name') ?? ''
    const surname = getValue(user, 'surname', 'Surname') ?? ''
    const fullName = `${name} ${surname}`.trim()
    return fullName || getValue(user, 'email', 'Email') || 'РљРѕСЂРёСЃС‚СѓРІР°С‡'
  }

  return String(getValue(member, 'userId', 'UserId'))
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
    <div className="page-stack">
      <section className="page-header">
        <div>
          <p className="eyebrow">Company</p>
          <h1>РљРµСЂСѓРІР°РЅРЅСЏ РєРѕРјРїР°РЅС–С”СЋ</h1>
        </div>
      </section>

      {error && <div className="api-error-banner"><span>{error}</span></div>}

      <section className="panel">
        <h2>РЎС‚РІРѕСЂРёС‚Рё РЅРѕРІСѓ РєРѕРјРїР°РЅС–СЋ</h2>
        <form className="form-grid" onSubmit={submitCreate}>
          <div className="form-section">
            <Input label="РќР°Р·РІР°" value={createName} onChange={(event) => setCreateName(event.target.value)} required />
            <Input label="РџРѕРґР°С‚РєРѕРІРёР№ РЅРѕРјРµСЂ" value={createTaxNumber} onChange={(event) => setCreateTaxNumber(event.target.value)} />
          </div>
          <div className="summary-panel">
            <Button type="submit" disabled={isBusy || !createName.trim()}>РЎС‚РІРѕСЂРёС‚Рё РєРѕРјРїР°РЅС–СЋ</Button>
          </div>
        </form>
      </section>

      <section className="panel">
        <h2>РџРѕС‚РѕС‡РЅР° РєРѕРјРїР°РЅС–СЏ</h2>
        <form className="form-grid" onSubmit={submitUpdate}>
          <div className="form-section">
            <Input label="РќР°Р·РІР°" value={editName} onChange={(event) => setEditName(event.target.value)} disabled={!canManageCompany} required />
            <Input label="РџРѕРґР°С‚РєРѕРІРёР№ РЅРѕРјРµСЂ" value={editTaxNumber ?? ''} onChange={(event) => setEditTaxNumber(event.target.value)} disabled={!canManageCompany} />
          </div>
          <div className="summary-panel company-actions">
            <Button type="submit" disabled={!canManageCompany || isBusy || !editName.trim()}>Р—Р±РµСЂРµРіС‚Рё Р·РјС–РЅРё</Button>
            <Button type="button" variant="secondary" disabled={!canManageCompany || isBusy} onClick={onDeleteCompany}>Р’РёРґР°Р»РёС‚Рё РєРѕРјРїР°РЅС–СЋ</Button>
          </div>
        </form>
      </section>

      <section className="panel">
        <h2>РЈС‡Р°СЃРЅРёРєРё РєРѕРјРїР°РЅС–С—</h2>

        {canManageCompany && (
          <form className="toolbar" onSubmit={submitAddMember}>
            <input
              type="email"
              placeholder="email РєРѕСЂРёСЃС‚СѓРІР°С‡Р°"
              value={memberEmail}
              onChange={(event) => setMemberEmail(event.target.value)}
              required
            />
            <Button type="submit" disabled={isBusy}>Р”РѕРґР°С‚Рё</Button>
          </form>
        )}

        <div className="table-header company-members-table">
          <span>РљРѕСЂРёСЃС‚СѓРІР°С‡</span>
          <span>Email</span>
          <span>Р РѕР»СЊ</span>
          <span>Р”С–С—</span>
        </div>

        {members.map((member) => {
          const memberUser = getValue(member, 'user', 'User')
          const memberUserId = String(getValue(member, 'userId', 'UserId'))
          const memberRole = Number(getValue(member, 'role', 'Role'))
          const isCurrentUser = memberUserId === String(currentUserId)
          const canModifyMember = canManageCompany && memberRole !== OWNER_ROLE

          return (
            <div className="table-row company-members-table" key={String(getValue(member, 'id', 'Id'))}>
              <span>{getMemberName(member)}</span>
              <span>{getValue(memberUser, 'email', 'Email') ?? '-'}</span>
              <span>
                {canModifyMember ? (
                  <select
                    value={memberRole}
                    onChange={(event) => onChangeMemberRole(memberUserId, Number(event.target.value))}
                    disabled={isBusy}
                  >
                    {roleOptions.map((role) => (
                      <option value={role.value} key={role.value}>{role.label}</option>
                    ))}
                  </select>
                ) : (
                  roleOptions.find((role) => role.value === memberRole)?.label ?? memberRole
                )}
              </span>
              <span>
                {canModifyMember && !isCurrentUser ? (
                  <Button type="button" variant="secondary" disabled={isBusy} onClick={() => onRemoveMember(memberUserId)}>
                    Р’РёРґР°Р»РёС‚Рё
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


