import { useState } from 'react'
import { Button } from '../components/Button.jsx'

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

export function CreateProductPage({ categories = [], warehouses = [], onBack, onCreate }) {
  const firstCategoryId = String(getValue(categories[0], 'id', 'Id') ?? '')
  const firstWarehouseId = String(getValue(warehouses[0], 'id', 'Id') ?? '')
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [weight, setWeight] = useState(0)
  const [productCategoryId, setProductCategoryId] = useState(firstCategoryId)
  const [warehouseId, setWarehouseId] = useState(firstWarehouseId)
  const [stock, setStock] = useState(1)
  const [cost, setCost] = useState(0)
  const [price, setPrice] = useState(0)
  const [color, setColor] = useState('-')
  const [size, setSize] = useState('-')
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
        name,
        description,
        weight,
        productCategoryId: selectedCategoryId,
        warehouseId: selectedWarehouseId,
        stock,
        cost,
        price,
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
          <div className="form-grid-3">
            <label className="field span-2">
              <span>Назва товару</span>
              <input required maxLength="100" value={name} onChange={(event) => setName(event.target.value)} />
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
              <span>Склад</span>
              <select required value={selectedWarehouseId} onChange={(event) => setWarehouseId(event.target.value)}>
                {warehouses.map((item) => (
                  <option key={getValue(item, 'id', 'Id')} value={getValue(item, 'id', 'Id')}>
                    {getValue(item, 'name', 'Name')}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Вага</span>
              <input min="0" step="0.01" type="number" value={weight} onChange={(event) => setWeight(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Кількість</span>
              <input min="0" type="number" value={stock} onChange={(event) => setStock(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Собівартість</span>
              <input min="0" step="0.01" type="number" value={cost} onChange={(event) => setCost(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Ціна продажу</span>
              <input min="0" step="0.01" type="number" value={price} onChange={(event) => setPrice(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Колір варіанту</span>
              <input required maxLength="50" value={color} onChange={(event) => setColor(event.target.value)} />
            </label>
            <label className="field">
              <span>Розмір варіанту</span>
              <input required maxLength="20" value={size} onChange={(event) => setSize(event.target.value)} />
            </label>
            <label className="field span-2">
              <span>Опис</span>
              <textarea required maxLength="500" rows="5" value={description} onChange={(event) => setDescription(event.target.value)} />
            </label>
          </div>

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
