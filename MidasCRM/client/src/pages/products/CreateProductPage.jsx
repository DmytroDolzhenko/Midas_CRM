import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

export function CreateProductPage({ categories = [], warehouses = [], onBack, onCreate }) {
  const firstWarehouseId = String(getValue(warehouses[0], 'id', 'Id') ?? '')
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [weight, setWeight] = useState('')
  const [productCategoryIds, setProductCategoryIds] = useState([])
  const [warehouseId, setWarehouseId] = useState(firstWarehouseId)
  const [quantity, setQuantity] = useState(1)
  const [costPrice, setCostPrice] = useState('')
  const [sellPrice, setSellPrice] = useState('')
  const [color, setColor] = useState('-')
  const [size, setSize] = useState('-')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const selectedWarehouseId = warehouseId || firstWarehouseId
  const selectedWarehouse = warehouses.find((item) => String(getValue(item, 'id', 'Id')) === String(selectedWarehouseId))
  const selectedCategories = categories.filter((item) => productCategoryIds.includes(Number(getValue(item, 'id', 'Id'))))

  const summary = useMemo(() => {
    const stock = Number(quantity) || 0
    const cost = Number(costPrice) || 0
    const price = Number(sellPrice) || 0

    return {
      costTotal: stock * cost,
      saleTotal: stock * price,
      profit: stock * (price - cost),
    }
  }, [costPrice, quantity, sellPrice])

  function toggleCategory(categoryId) {
    setProductCategoryIds((currentCategoryIds) =>
      currentCategoryIds.includes(categoryId)
        ? currentCategoryIds.filter((item) => item !== categoryId)
        : [...currentCategoryIds, categoryId],
    )
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!selectedWarehouseId) {
      setError('РЎС‚РІРѕСЂС–С‚СЊ Р°Р±Рѕ РѕР±РµСЂС–С‚СЊ СЃРєР»Р°Рґ РґР»СЏ С‚РѕРІР°СЂСѓ')
      return
    }

    if (!name.trim() || !description.trim()) {
      setError('Р—Р°РїРѕРІРЅС–С‚СЊ РЅР°Р·РІСѓ С‚Р° РѕРїРёСЃ С‚РѕРІР°СЂСѓ')
      return
    }

    if (productCategoryIds.length === 0) {
      setError('РћР±РµСЂС–С‚СЊ С…РѕС‡Р° Р± РѕРґРЅСѓ РєР°С‚РµРіРѕСЂС–СЋ')
      return
    }

    setIsSubmitting(true)

    try {
      await onCreate({
        name: name.trim(),
        description: description.trim(),
        weight: Number(weight) || 0,
        productCategoryIds,
        warehouseId: selectedWarehouseId,
        stock: Number(quantity) || 0,
        cost: Number(costPrice) || 0,
        price: Number(sellPrice) || 0,
        color: color.trim() || '-',
        size: size.trim() || '-',
      })
    } catch (submitError) {
      setError(submitError.message || 'РќРµ РІРґР°Р»РѕСЃСЏ СЃС‚РІРѕСЂРёС‚Рё С‚РѕРІР°СЂ')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="page-stack product-create-page">
      <div className="page-header product-create-header">
        <div>
          <p className="eyebrow">Catalog</p>
          <h1>РќРѕРІРёР№ С‚РѕРІР°СЂ</h1>
          <p>Р—Р°РїРѕРІРЅС–С‚СЊ РѕСЃРЅРѕРІРЅС– РґР°РЅС–, СЃРєР»Р°Рґ С– РїРµСЂС€РёР№ РІР°СЂС–Р°РЅС‚ С‚РѕРІР°СЂСѓ. Р¦С– РїРѕР»СЏ РІС–РґРїРѕРІС–РґР°СЋС‚СЊ СЃРµСЂРІРµСЂРЅРѕРјСѓ `product-with-variants`.</p>
        </div>
        <Button variant="secondary" onClick={onBack}>Р”Рѕ РєР°С‚Р°Р»РѕРіСѓ</Button>
      </div>

      <form className="product-create-layout" onSubmit={handleSubmit}>
        <section className="panel product-form-panel">
          <div className="product-form-section">
            <div>
              <p className="eyebrow">Product</p>
              <h2>РћСЃРЅРѕРІРЅР° С–РЅС„РѕСЂРјР°С†С–СЏ</h2>
            </div>
            <div className="form-grid-3">
              <label className="field span-2">
                <span>РќР°Р·РІР° С‚РѕРІР°СЂСѓ</span>
                <input required maxLength="100" value={name} onChange={(event) => setName(event.target.value)} placeholder="РќР°РїСЂРёРєР»Р°Рґ, Oversize hoodie" />
              </label>
              <label className="field">
                <span>Р’Р°РіР°, РєРі</span>
                <input min="0" step="0.01" type="number" value={weight} onChange={(event) => setWeight(event.target.value)} placeholder="0.4" />
              </label>
              <label className="field span-2">
                <span>РћРїРёСЃ</span>
                <textarea required maxLength="500" rows="4" value={description} onChange={(event) => setDescription(event.target.value)} placeholder="РљРѕСЂРѕС‚РєРёР№ РѕРїРёСЃ С‚РѕРІР°СЂСѓ РґР»СЏ РІРЅСѓС‚СЂС–С€РЅСЊРѕРіРѕ РєР°С‚Р°Р»РѕРіСѓ" />
              </label>
              <label className="field">
                <span>РЎРєР»Р°Рґ</span>
                <select required value={selectedWarehouseId} onChange={(event) => setWarehouseId(event.target.value)}>
                  <option value="">РћР±РµСЂС–С‚СЊ СЃРєР»Р°Рґ</option>
                  {warehouses.map((item) => (
                    <option key={getValue(item, 'id', 'Id')} value={getValue(item, 'id', 'Id')}>
                      {getValue(item, 'name', 'Name')}
                    </option>
                  ))}
                </select>
              </label>
            </div>
          </div>

          <div className="product-form-section">
            <div>
              <p className="eyebrow">Categories</p>
              <h2>РљР°С‚РµРіРѕСЂС–С—</h2>
            </div>
            <div className="checkbox-grid">
              {categories.map((item) => {
                const categoryId = Number(getValue(item, 'id', 'Id'))
                return (
                  <label key={categoryId} className="checkbox-row">
                    <input
                      type="checkbox"
                      checked={productCategoryIds.includes(categoryId)}
                      onChange={() => toggleCategory(categoryId)}
                    />
                    {getValue(item, 'name', 'Name')}
                  </label>
                )
              })}
            </div>
          </div>

          <div className="product-form-section">
            <div>
              <p className="eyebrow">Variant</p>
              <h2>РџРµСЂС€РёР№ РІР°СЂС–Р°РЅС‚</h2>
            </div>
            <div className="form-grid-3">
              <label className="field">
                <span>РљРѕР»С–СЂ</span>
                <input required maxLength="50" value={color} onChange={(event) => setColor(event.target.value)} />
              </label>
              <label className="field">
                <span>Р РѕР·РјС–СЂ</span>
                <input required maxLength="20" value={size} onChange={(event) => setSize(event.target.value)} />
              </label>
              <label className="field">
                <span>РљС–Р»СЊРєС–СЃС‚СЊ</span>
                <input min="0" type="number" value={quantity} onChange={(event) => setQuantity(event.target.value)} />
              </label>
              <label className="field">
                <span>РЎРѕР±С–РІР°СЂС‚С–СЃС‚СЊ</span>
                <input min="0" step="0.01" type="number" value={costPrice} onChange={(event) => setCostPrice(event.target.value)} />
              </label>
              <label className="field">
                <span>Р¦С–РЅР° РїСЂРѕРґР°Р¶Сѓ</span>
                <input min="0" step="0.01" type="number" value={sellPrice} onChange={(event) => setSellPrice(event.target.value)} />
              </label>
            </div>
          </div>

          {error && <p className="form-error">{error}</p>}
        </section>

        <aside className="panel product-summary-card">
          <p className="eyebrow">Preview</p>
          <h2>{name || 'РќРѕРІРёР№ С‚РѕРІР°СЂ'}</h2>
          <p>{description || 'РћРїРёСЃ Р·КјСЏРІРёС‚СЊСЃСЏ С‚СѓС‚ РїС–СЃР»СЏ Р·Р°РїРѕРІРЅРµРЅРЅСЏ С„РѕСЂРјРё.'}</p>

          <div className="summary-pill-list">
            <span>{selectedWarehouse ? getValue(selectedWarehouse, 'name', 'Name') : 'РЎРєР»Р°Рґ РЅРµ РѕР±СЂР°РЅРѕ'}</span>
            <span>{selectedCategories.length ? selectedCategories.map((item) => getValue(item, 'name', 'Name')).join(', ') : 'Р‘РµР· РєР°С‚РµРіРѕСЂС–С—'}</span>
            <span>{color || '-'} / {size || '-'}</span>
          </div>

          <div className="summary-line">
            <span>РЎРѕР±С–РІР°СЂС‚С–СЃС‚СЊ Р·Р°Р»РёС€РєСѓ</span>
            <strong>{summary.costTotal.toLocaleString('uk-UA')} РіСЂРЅ</strong>
          </div>
          <div className="summary-line">
            <span>РџРѕС‚РµРЅС†С–Р№РЅРёР№ РїСЂРѕРґР°Р¶</span>
            <strong>{summary.saleTotal.toLocaleString('uk-UA')} РіСЂРЅ</strong>
          </div>
          <div className="summary-total">
            <span>РџРѕС‚РµРЅС†С–Р№РЅРёР№ РїСЂРёР±СѓС‚РѕРє</span>
            <strong>{summary.profit.toLocaleString('uk-UA')} РіСЂРЅ</strong>
          </div>

          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'РЎС‚РІРѕСЂРµРЅРЅСЏ...' : 'РЎС‚РІРѕСЂРёС‚Рё С‚РѕРІР°СЂ'}
          </Button>
        </aside>
      </form>
    </section>
  )
}


