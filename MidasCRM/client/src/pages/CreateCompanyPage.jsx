import { useState } from 'react'
import { Button } from '../components/Button.jsx'
import { Input } from '../components/Input.jsx'

export function CreateCompanyPage({ userEmail, onCreateCompany, isSubmitting, error, onLogout }) {
  const [name, setName] = useState('')
  const [taxNumber, setTaxNumber] = useState('')
  const [formError, setFormError] = useState('')

  async function handleSubmit(event) {
    event.preventDefault()

    if (!name.trim()) {
      setFormError('Р’РєР°Р¶С–С‚СЊ РЅР°Р·РІСѓ РєРѕРјРїР°РЅС–С—')
      return
    }

    setFormError('')

    await onCreateCompany({
      name: name.trim(),
      taxNumber: taxNumber.trim() || null,
    })
  }

  return (
    <main className="login-page">
      <form className="login-card" onSubmit={handleSubmit}>
        <div>
          <p className="eyebrow">Midas CRM</p>
          <h1>РЎС‚РІРѕСЂРёС‚Рё РєРѕРјРїР°РЅС–СЋ</h1>
          <p className="create-company-subtitle">
            {userEmail} РЅРµ С” СѓС‡Р°СЃРЅРёРєРѕРј Р¶РѕРґРЅРѕС— РєРѕРјРїР°РЅС–С—. РЎС‚РІРѕСЂС–С‚СЊ РєРѕРјРїР°РЅС–СЋ, С‰РѕР± РїСЂРѕРґРѕРІР¶РёС‚Рё СЂРѕР±РѕС‚Сѓ.
          </p>
        </div>

        <Input
          label="РќР°Р·РІР° РєРѕРјРїР°РЅС–С—"
          value={name}
          onChange={(event) => setName(event.target.value)}
          placeholder="РўРћР’ РњС–РґР°СЃ"
          required
        />

        <Input
          label="РџРѕРґР°С‚РєРѕРІРёР№ РЅРѕРјРµСЂ (РЅРµРѕР±РѕРІ'СЏР·РєРѕРІРѕ)"
          value={taxNumber}
          onChange={(event) => setTaxNumber(event.target.value)}
          placeholder="12345678"
        />

        {(formError || error) && <p className="form-error">{formError || error}</p>}

        <Button className="full-width" type="submit" disabled={isSubmitting}>
          {isSubmitting ? 'РЎС‚РІРѕСЂРµРЅРЅСЏ...' : 'РЎС‚РІРѕСЂРёС‚Рё РєРѕРјРїР°РЅС–СЋ'}
        </Button>

        <Button className="full-width" type="button" variant="secondary" onClick={onLogout}>
          Р’РёР№С‚Рё
        </Button>
      </form>
    </main>
  )
}