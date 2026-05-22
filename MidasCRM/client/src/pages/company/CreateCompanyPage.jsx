import { useState } from 'react'
import { Button } from '../../components/Button.jsx'
import { Input } from '../../components/Input.jsx'

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
    <main className="company-setup-page">
      <section className="company-setup-card">
        <div className="company-setup-copy">
          <p className="eyebrow">РџРµСЂС€РёР№ Р·Р°РїСѓСЃРє</p>
          <h1>РЎС‚РІРѕСЂС–С‚СЊ РєРѕРјРїР°РЅС–СЋ</h1>
          <p>
            РљРѕРјРїР°РЅС–СЏ РѕР±КјС”РґРЅСѓС” С‚РѕРІР°СЂРё, РїСЂРѕРґР°Р¶С–, РєР»С–С”РЅС‚С–РІ С– С„С–РЅР°РЅСЃРё РІ РѕРґРЅРѕРјСѓ СЂРѕР±РѕС‡РѕРјСѓ РїСЂРѕСЃС‚РѕСЂС–.
            РџС–СЃР»СЏ СЃС‚РІРѕСЂРµРЅРЅСЏ РІРё РѕРґСЂР°Р·Сѓ РїРµСЂРµР№РґРµС‚Рµ РґРѕ CRM.
          </p>
          <div className="company-user-chip">{userEmail}</div>
        </div>

        <form className="company-setup-form" onSubmit={handleSubmit}>
          <Input
            label="РќР°Р·РІР° РєРѕРјРїР°РЅС–С—"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="РќР°РїСЂРёРєР»Р°Рґ, Gachi Store"
            required
          />

          <Input
            label="РџРѕРґР°С‚РєРѕРІРёР№ РЅРѕРјРµСЂ"
            value={taxNumber}
            onChange={(event) => setTaxNumber(event.target.value)}
            placeholder="РќРµРѕР±РѕРІКјСЏР·РєРѕРІРѕ"
          />

          {(formError || error) && <p className="form-error">{formError || error}</p>}

          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'РЎС‚РІРѕСЂСЋС”РјРѕ...' : 'РЎС‚РІРѕСЂРёС‚Рё РєРѕРјРїР°РЅС–СЋ'}
          </Button>

          <Button className="full-width" type="button" variant="secondary" onClick={onLogout}>
            Р’РёР№С‚Рё
          </Button>
        </form>
      </section>
    </main>
  )
}

