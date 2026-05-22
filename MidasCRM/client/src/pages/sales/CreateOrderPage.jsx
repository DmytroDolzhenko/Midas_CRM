import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'

const serviceTypes = [
  { id: 0, label: 'Р”РІРµСЂС– - РґРІРµСЂС–' },
  { id: 1, label: 'Р”РІРµСЂС– - СЃРєР»Р°Рґ' },
  { id: 2, label: 'РЎРєР»Р°Рґ - СЃРєР»Р°Рґ' },
  { id: 3, label: 'РЎРєР»Р°Рґ - РґРІРµСЂС–' },
]

const cargoTypes = [
  { id: 1, label: 'Р’Р°РЅС‚Р°Р¶' },
  { id: 2, label: 'Р”РѕРєСѓРјРµРЅС‚Рё' },
  { id: 3, label: 'РџРѕСЃРёР»РєР°' },
]

const paymentMethods = [
  { id: 0, label: 'РџРѕРІРЅР° РѕРїР»Р°С‚Р°' },
  { id: 1, label: 'РџС–СЃР»СЏРїР»Р°С‚Р°' },
  { id: 2, label: 'РћРїР»Р°С‡СѓС” РІС–РґРїСЂР°РІРЅРёРє' },
]

export function CreateOrderPage({ customers, products, onBack, onCreate }) {
  const [productQuery, setProductQuery] = useState('')
  const [isProductPickerOpen, setIsProductPickerOpen] = useState(false)
  const [productId, setProductId] = useState(String(products[0]?.id ?? ''))
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [quantity, setQuantity] = useState(1)
  const [city, setCity] = useState('РљРёС—РІ')
  const [postalCode, setPostalCode] = useState(1)
  const [postDepartmentNumber, setPostDepartmentNumber] = useState(1)
  const [serviceType, setServiceType] = useState(2)
  const [cargoType, setCargoType] = useState(1)
  const [paymentMethod, setPaymentMethod] = useState(1)
  const [description, setDescription] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const selectedProductId = productId || String(products[0]?.id ?? '')
  const selectedCustomerId = customerId || String(customers[0]?.id ?? '')

  const filteredProducts = useMemo(
    () =>
      products.filter((product) =>
        `${product.name} ${product.sku} ${product.category} ${product.warehouse}`
          .toLowerCase()
          .includes(productQuery.toLowerCase()),
      ),
    [productQuery, products],
  )
  const selectedProduct = products.find((product) => product.id === Number(selectedProductId))
  const subtotal = (selectedProduct?.price ?? 0) * quantity
  const orderDescription = description.trim() || selectedProduct?.name || 'CRM order'

  function chooseProduct(product) {
    setProductId(String(product.id))
    setProductQuery('')
    setIsProductPickerOpen(false)
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await onCreate({
        productId: selectedProductId,
        customerId: selectedCustomerId,
        quantity,
        city,
        postalCode,
        postDepartmentNumber,
        serviceType,
        cargoType,
        paymentMethods: paymentMethod,
        description: orderDescription,
      })
    } catch (submitError) {
      setError(submitError.message || 'РќРµ РІРґР°Р»РѕСЃСЏ СЃС‚РІРѕСЂРёС‚Рё РїСЂРѕРґР°Р¶')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Sales</p>
          <h1>РќРѕРІРёР№ РїСЂРѕРґР°Р¶</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>Р”Рѕ РїСЂРѕРґР°Р¶С–РІ</Button>
      </div>

      <form className="wide-form" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <div className="form-grid-3">
            <label className="field span-2">
              <span>РўРѕРІР°СЂ</span>
              <button className="product-picker-button" type="button" onClick={() => setIsProductPickerOpen(true)}>
                <strong>{selectedProduct?.name ?? 'РћР±РµСЂС–С‚СЊ С‚РѕРІР°СЂ'}</strong>
                <small>{selectedProduct ? `${selectedProduct.sku} В· ${selectedProduct.warehouse}` : 'Р’С–РґРєСЂРёС‚Рё РєР°С‚Р°Р»РѕРі С‚РѕРІР°СЂС–РІ'}</small>
              </button>
            </label>
            <label className="field">
              <span>РљР»С–С”РЅС‚</span>
              <select required value={selectedCustomerId} onChange={(event) => setCustomerId(event.target.value)}>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>РљС–Р»СЊРєС–СЃС‚СЊ</span>
              <input min="1" type="number" value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} />
            </label>
          </div>

          <div className="delivery-box">
            <strong>РџР°СЂР°РјРµС‚СЂРё Р·Р°РјРѕРІР»РµРЅРЅСЏ</strong>
            <div className="form-grid-3">
              <label className="field">
                <span>РўРёРї СЃРµСЂРІС–СЃСѓ</span>
                <select value={serviceType} onChange={(event) => setServiceType(Number(event.target.value))}>
                  {serviceTypes.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>РўРёРї РІР°РЅС‚Р°Р¶Сѓ</span>
                <select value={cargoType} onChange={(event) => setCargoType(Number(event.target.value))}>
                  {cargoTypes.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>РћРїР»Р°С‚Р°</span>
                <select value={paymentMethod} onChange={(event) => setPaymentMethod(Number(event.target.value))}>
                  {paymentMethods.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field span-2">
                <span>РћРїРёСЃ</span>
                <textarea rows="3" value={description} onChange={(event) => setDescription(event.target.value)} placeholder={selectedProduct?.name ?? ''} />
              </label>
            </div>
          </div>

          <div className="delivery-box">
            <strong>РђРґСЂРµСЃР° РґРѕСЃС‚Р°РІРєРё</strong>
            <div className="form-grid-3">
              <label className="field">
                <span>РњС–СЃС‚Рѕ</span>
                <input required maxLength="100" value={city} onChange={(event) => setCity(event.target.value)} />
              </label>
              <label className="field">
                <span>РџРѕС€С‚РѕРІРёР№ РєРѕРґ</span>
                <input min="1" type="number" value={postalCode} onChange={(event) => setPostalCode(Number(event.target.value))} />
              </label>
              <label className="field">
                <span>Р’С–РґРґС–Р»РµРЅРЅСЏ РќРџ</span>
                <input min="1" type="number" value={postDepartmentNumber} onChange={(event) => setPostDepartmentNumber(Number(event.target.value))} />
              </label>
            </div>
          </div>

          {error && <p className="form-error">{error}</p>}
        </section>

        <section className="panel summary-panel">
          <h2>РџС–РґСЃСѓРјРѕРє РїСЂРѕРґР°Р¶Сѓ</h2>
          <div className="summary-line">
            <span>РўРѕРІР°СЂ</span>
            <strong>{selectedProduct?.name ?? '-'}</strong>
          </div>
          <div className="summary-line">
            <span>РљС–Р»СЊРєС–СЃС‚СЊ</span>
            <strong>{quantity}</strong>
          </div>
          <div className="summary-total">
            <span>РћСЂС–С”РЅС‚РѕРІРЅР° СЃСѓРјР°</span>
            <strong>{subtotal.toLocaleString('uk-UA')} РіСЂРЅ</strong>
          </div>
          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'РЎС‚РІРѕСЂРµРЅРЅСЏ...' : 'РЎС‚РІРѕСЂРёС‚Рё РїСЂРѕРґР°Р¶'}
          </Button>
        </section>
      </form>

      {isProductPickerOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsProductPickerOpen(false)}>
          <section className="product-picker-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Catalog</p>
                <h2>РћР±РµСЂС–С‚СЊ С‚РѕРІР°СЂ</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setIsProductPickerOpen(false)}>x</button>
            </div>
            <div className="product-picker-search">
              <input value={productQuery} onChange={(event) => setProductQuery(event.target.value)} placeholder="РџРѕС€СѓРє Р·Р° РЅР°Р·РІРѕСЋ, Р°СЂС‚РёРєСѓР»РѕРј Р°Р±Рѕ СЃРєР»Р°РґРѕРј" />
            </div>
            <div className="product-picker-list">
              {filteredProducts.map((product) => (
                <button key={product.id} type="button" onClick={() => chooseProduct(product)}>
                  <span>
                    <strong>{product.name}</strong>
                    <small>{product.sku} В· {product.category} В· {product.warehouse}</small>
                  </span>
                  <b>{product.price.toLocaleString('uk-UA')} РіСЂРЅ</b>
                </button>
              ))}
            </div>
          </section>
        </div>
      )}
    </section>
  )
}

