import { useState } from 'react'
import { Button } from '../components/Button.jsx'

const productTypes = [
  { id: 'default', label: 'Звичайний' },
  { id: 'variable', label: 'Варіативний' },
  { id: 'set', label: 'Комплект' },
  { id: 'service', label: 'Послуги' },
]

const brands = ['Midas', 'AquaLine', 'Gorpcore Lab', 'Nova Brand']
const units = ['одиниць', 'комплектів', 'пар', 'послуг']

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function generateSku(name) {
  const prefix = name.trim().slice(0, 3).toUpperCase() || 'PRD'
  return `${prefix}-${Math.floor(1000 + Math.random() * 9000)}`
}

function generateBarcode() {
  return `482${Math.floor(100000000 + Math.random() * 900000000)}`
}

export function CreateProductPage({ categories = [], warehouses = [], onBack, onCreate }) {
  const firstCategoryId = String(getValue(categories[0], 'id', 'Id') ?? '')
  const firstWarehouseId = String(getValue(warehouses[0], 'id', 'Id') ?? '')
  const [type, setType] = useState('default')
  const [name, setName] = useState('')
  const [productCategoryId, setProductCategoryId] = useState(firstCategoryId)
  const [brand, setBrand] = useState(brands[0])
  const [unit, setUnit] = useState('одиниць')
  const [barcode, setBarcode] = useState('')
  const [sku, setSku] = useState('')
  const [warehouseId, setWarehouseId] = useState(firstWarehouseId)
  const [stock, setStock] = useState(1)
  const [cost, setCost] = useState(0)
  const [price, setPrice] = useState(0)
  const [description, setDescription] = useState('')
  const [color, setColor] = useState('-')
  const [size, setSize] = useState('-')
  const [isExtraOpen, setIsExtraOpen] = useState(false)
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const selectedCategoryId = productCategoryId || firstCategoryId
  const selectedWarehouseId = warehouseId || firstWarehouseId

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await onCreate({
        type,
        name,
        productCategoryId: selectedCategoryId,
        brand,
        unit,
        barcode,
        sku,
        warehouseId: selectedWarehouseId,
        stock,
        cost,
        price,
        description,
        color,
        size,
      })
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося створити товар')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <h1>Новий товар</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>
          До каталогу
        </Button>
      </div>

      <form className="wide-form" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <div className="tabs">
            {productTypes.map((item) => (
              <button
                key={item.id}
                type="button"
                className={item.id === type ? 'tab-button active' : 'tab-button'}
                onClick={() => setType(item.id)}
              >
                {item.label}
              </button>
            ))}
          </div>

          <div className="form-grid-3">
            <label className="field span-2">
              <span>Назва нового товару</span>
              <input required value={name} onChange={(event) => setName(event.target.value)} />
            </label>
            <label className="field">
              <span>Категорія</span>
              <select required value={selectedCategoryId} onChange={(event) => setProductCategoryId(event.target.value)}>
                {categories.map((item) => (
                  <option key={getValue(item, 'id', 'Id')} value={getValue(item, 'id', 'Id')}>
                    {getValue(item, 'name', 'Name')}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Бренд</span>
              <select value={brand} onChange={(event) => setBrand(event.target.value)}>
                {brands.map((item) => (
                  <option key={item}>{item}</option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Од. виміру</span>
              <select value={unit} onChange={(event) => setUnit(event.target.value)}>
                {units.map((item) => (
                  <option key={item}>{item}</option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Склад</span>
              <select required value={selectedWarehouseId} onChange={(event) => setWarehouseId(event.target.value)}>
                {warehouses.map((item) => (
                  <option key={getValue(item, 'id', 'Id')} value={getValue(item, 'id', 'Id')}>
                    {getValue(item, 'name', 'Name')}
                  </option>
                ))}
              </select>
            </label>
            <label className="field input-with-action">
              <span>Штрих-код</span>
              <div>
                <input value={barcode} onChange={(event) => setBarcode(event.target.value)} />
                <button type="button" onClick={() => setBarcode(generateBarcode())}>
                  Згенерувати
                </button>
              </div>
            </label>
            <label className="field input-with-action">
              <span>Артикул</span>
              <div>
                <input value={sku} onChange={(event) => setSku(event.target.value)} />
                <button type="button" onClick={() => setSku(generateSku(name))}>
                  Згенерувати
                </button>
              </div>
            </label>
            <label className="field">
              <span>Од. на складі</span>
              <input min="0" type="number" value={stock} onChange={(event) => setStock(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Собівартість</span>
              <input min="0" type="number" value={cost} onChange={(event) => setCost(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Ціна продажу</span>
              <input min="0" type="number" value={price} onChange={(event) => setPrice(Number(event.target.value))} />
            </label>
          </div>

          <button className="expand-button" type="button" onClick={() => setIsExtraOpen((isOpen) => !isOpen)}>
            Додаткова інформація {isExtraOpen ? 'Згорнути' : 'Розгорнути'}
          </button>

          {isExtraOpen && (
            <div className="form-grid-3">
              <label className="field span-2">
                <span>Опис</span>
                <textarea rows="5" value={description} onChange={(event) => setDescription(event.target.value)} />
              </label>
              <label className="field">
                <span>Колір варіанту</span>
                <input value={color} onChange={(event) => setColor(event.target.value)} />
              </label>
              <label className="field">
                <span>Розмір варіанту</span>
                <input value={size} onChange={(event) => setSize(event.target.value)} />
              </label>
            </div>
          )}

          {error && <p className="form-error">{error}</p>}
        </section>

        <section className="panel summary-panel">
          <h2>Підсумок товару</h2>
          <div className="summary-line">
            <span>Собівартість залишку</span>
            <strong>{(stock * cost).toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-line">
            <span>Потенційний продаж</span>
            <strong>{(stock * price).toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-total">
            <span>Потенційний прибуток</span>
            <strong>{(stock * (price - cost)).toLocaleString('uk-UA')} грн</strong>
          </div>
          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створення...' : 'Створити товар'}
          </Button>
        </section>
      </form>
    </section>
  )
}
