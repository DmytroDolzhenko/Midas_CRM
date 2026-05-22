import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function createEmptyVariant() {
  return {
    color: '-',
    size: '-',
    quantity: 1,
    costPrice: '',
    sellPrice: '',
  }
}

export function CreateProductPage({ categories = [], warehouses = [], onBack, onCreate }) {
  const firstWarehouseId = String(getValue(warehouses[0], 'id', 'Id') ?? '')
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')
  const [weight, setWeight] = useState('')
  const [productCategoryIds, setProductCategoryIds] = useState([])
  const [warehouseId, setWarehouseId] = useState(firstWarehouseId)
  const [currentVariant, setCurrentVariant] = useState(createEmptyVariant)
  const [variants, setVariants] = useState([])
  const [images, setImages] = useState([])
  const [mainImageIndex, setMainImageIndex] = useState(0)
  const [error, setError] = useState('')
  const [notice, setNotice] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const selectedWarehouseId = warehouseId || firstWarehouseId
  const selectedWarehouse = warehouses.find((item) => String(getValue(item, 'id', 'Id')) === String(selectedWarehouseId))
  const selectedCategories = categories.filter((item) => productCategoryIds.includes(Number(getValue(item, 'id', 'Id'))))

  const summary = useMemo(() => {
    const totals = variants.reduce(
      (accumulator, variant) => {
        const stock = Number(variant.quantity) || 0
        const cost = Number(variant.costPrice) || 0
        const price = Number(variant.sellPrice) || 0

        return {
          costTotal: accumulator.costTotal + stock * cost,
          saleTotal: accumulator.saleTotal + stock * price,
          quantityTotal: accumulator.quantityTotal + stock,
        }
      },
      { costTotal: 0, saleTotal: 0, quantityTotal: 0 },
    )

    return {
      ...totals,
      profit: totals.saleTotal - totals.costTotal,
    }
  }, [variants])

  function toggleCategory(categoryId) {
    setProductCategoryIds((currentCategoryIds) =>
      currentCategoryIds.includes(categoryId)
        ? currentCategoryIds.filter((item) => item !== categoryId)
        : [...currentCategoryIds, categoryId],
    )
  }

  function updateCurrentVariant(field, value) {
    setCurrentVariant((current) => ({ ...current, [field]: value }))
  }

  function addVariant() {
    const color = String(currentVariant.color ?? '').trim() || '-'
    const size = String(currentVariant.size ?? '').trim() || '-'
    const quantity = Number(currentVariant.quantity) || 0
    const costPrice = Number(currentVariant.costPrice) || 0
    const sellPrice = Number(currentVariant.sellPrice) || 0

    if (quantity <= 0) {
      setError('Кількість варіанту має бути більшою за 0')
      return
    }

    if (costPrice < 0 || sellPrice < 0) {
      setError('Собівартість та ціна продажу не можуть бути відʼємними')
      return
    }

    setError('')
    setVariants((current) => [
      ...current,
      {
        id: crypto.randomUUID(),
        color,
        size,
        quantity,
        costPrice,
        sellPrice,
      },
    ])
    setCurrentVariant(createEmptyVariant())
  }

  function removeVariant(variantId) {
    setVariants((current) => current.filter((item) => item.id !== variantId))
  }

  function handleImageChange(event) {
    const selectedFiles = Array.from(event.target.files ?? [])
    if (!selectedFiles.length) {
      return
    }

    setImages((current) => [...current, ...selectedFiles])
    setNotice('')
  }

  function removeImage(indexToRemove) {
    setImages((current) => current.filter((_, index) => index !== indexToRemove))
    setMainImageIndex((currentMainIndex) => {
      if (indexToRemove === currentMainIndex) {
        return 0
      }

      if (indexToRemove < currentMainIndex) {
        return currentMainIndex - 1
      }

      return currentMainIndex
    })
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setNotice('')

    if (!selectedWarehouseId) {
      setError('Створіть або оберіть склад для товару')
      return
    }

    if (!name.trim() || !description.trim()) {
      setError('Заповніть назву та опис товару')
      return
    }

    if (productCategoryIds.length === 0) {
      setError('Оберіть хоча б одну категорію')
      return
    }

    if (variants.length === 0) {
      setError('Додайте хоча б один варіант товару кнопкою "Додати ще один варіант"')
      return
    }

    setIsSubmitting(true)

    try {
      const creationResult = await onCreate({
        name: name.trim(),
        description: description.trim(),
        weight: Number(weight) || 0,
        productCategoryIds,
        warehouseId: selectedWarehouseId,
        variants: variants.map((variant) => ({
          color: variant.color,
          size: variant.size,
          quantity: Number(variant.quantity) || 0,
          costPrice: Number(variant.costPrice) || 0,
          sellPrice: Number(variant.sellPrice) || 0,
        })),
        images,
        mainImageIndex,
      })

      if (creationResult?.failedCount > 0) {
        setNotice(`Товар створено, але ${creationResult.failedCount} фото не вдалося завантажити.`)
      }
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося створити товар')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="page-stack product-create-page">
      <div className="page-header product-create-header">
        <div>
          <p className="eyebrow">Catalog</p>
          <h1>Новий товар</h1>
          <p>Заповніть основні дані, склад і додайте один або кілька варіантів товару.</p>
        </div>
        <Button variant="secondary" onClick={onBack}>До каталогу</Button>
      </div>

      <form className="product-create-layout" onSubmit={handleSubmit}>
        <section className="panel product-form-panel">
          <div className="product-form-section">
            <div>
              <p className="eyebrow">Product</p>
              <h2>Основна інформація</h2>
            </div>
            <div className="form-grid-3">
              <label className="field span-2">
                <span>Назва товару</span>
                <input required maxLength="100" value={name} onChange={(event) => setName(event.target.value)} placeholder="Наприклад, Oversize hoodie" />
              </label>
              <label className="field">
                <span>Вага, кг</span>
                <input min="0" step="0.01" type="number" value={weight} onChange={(event) => setWeight(event.target.value)} placeholder="0.4" />
              </label>
              <label className="field span-2">
                <span>Опис</span>
                <textarea required maxLength="500" rows="4" value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Короткий опис товару для внутрішнього каталогу" />
              </label>
              <label className="field">
                <span>Склад</span>
                <select required value={selectedWarehouseId} onChange={(event) => setWarehouseId(event.target.value)}>
                  <option value="">Оберіть склад</option>
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
              <h2>Категорії</h2>
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
              <h2>Додати варіант товару</h2>
            </div>
            <div className="form-grid-3">
              <label className="field">
                <span>Колір</span>
                <input required maxLength="50" value={currentVariant.color} onChange={(event) => updateCurrentVariant('color', event.target.value)} />
              </label>
              <label className="field">
                <span>Розмір</span>
                <input required maxLength="20" value={currentVariant.size} onChange={(event) => updateCurrentVariant('size', event.target.value)} />
              </label>
              <label className="field">
                <span>Кількість</span>
                <input min="1" type="number" value={currentVariant.quantity} onChange={(event) => updateCurrentVariant('quantity', event.target.value)} />
              </label>
              <label className="field">
                <span>Собівартість</span>
                <input min="0" step="0.01" type="number" value={currentVariant.costPrice} onChange={(event) => updateCurrentVariant('costPrice', event.target.value)} />
              </label>
              <label className="field">
                <span>Ціна продажу</span>
                <input min="0" step="0.01" type="number" value={currentVariant.sellPrice} onChange={(event) => updateCurrentVariant('sellPrice', event.target.value)} />
              </label>
            </div>
            <Button type="button" variant="secondary" onClick={addVariant}>Додати ще один варіант</Button>

            <div className="panel" style={{ padding: '16px', marginTop: '8px' }}>
              <h3 style={{ margin: '0 0 12px' }}>Варіанти товару ({variants.length})</h3>
              {variants.length === 0 ? (
                <p style={{ margin: 0 }}>Ще не додано жодного варіанту</p>
              ) : (
                <div>
                  <div className="table-header product-table">
                    <span>Колір</span>
                    <span>Розмір</span>
                    <span>Кількість</span>
                    <span>Собівартість</span>
                    <span>Ціна продажу</span>
                  </div>
                  {variants.map((variant) => (
                    <div key={variant.id} className="table-row product-table">
                      <span>{variant.color}</span>
                      <span>{variant.size}</span>
                      <span>{variant.quantity}</span>
                      <span>{variant.costPrice.toLocaleString('uk-UA')} грн</span>
                      <span>
                        {variant.sellPrice.toLocaleString('uk-UA')} грн{' '}
                        <button type="button" onClick={() => removeVariant(variant.id)}>Видалити</button>
                      </span>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          <div className="product-form-section">
            <div>
              <p className="eyebrow">Images</p>
              <h2>Фото товару</h2>
            </div>
            <label className="field">
              <span>Додати фото</span>
              <input multiple accept="image/*" type="file" onChange={handleImageChange} />
            </label>

            <div className="panel" style={{ padding: '16px', marginTop: '8px' }}>
              <h3 style={{ margin: '0 0 12px' }}>Додані фото ({images.length})</h3>
              {images.length === 0 ? (
                <p style={{ margin: 0 }}>Фото не додано. Товар можна створити і без фото.</p>
              ) : (
                <div className="table-header product-table" style={{ gridTemplateColumns: '1.4fr 1fr 0.8fr 0.8fr 0.7fr' }}>
                  <span>Файл</span>
                  <span>Розмір</span>
                  <span>Головне</span>
                  <span>Превʼю</span>
                  <span>Дії</span>
                </div>
              )}

              {images.map((image, index) => (
                <div key={`${image.name}-${index}`} className="table-row product-table" style={{ gridTemplateColumns: '1.4fr 1fr 0.8fr 0.8fr 0.7fr' }}>
                  <span>{image.name}</span>
                  <span>{(image.size / 1024 / 1024).toFixed(2)} MB</span>
                  <span>
                    <input checked={mainImageIndex === index} name="main-image" type="radio" onChange={() => setMainImageIndex(index)} />
                  </span>
                  <span>
                    <img alt={image.name} src={URL.createObjectURL(image)} style={{ width: 40, height: 40, objectFit: 'cover', borderRadius: 8 }} />
                  </span>
                  <span>
                    <button type="button" onClick={() => removeImage(index)}>Видалити</button>
                  </span>
                </div>
              ))}
            </div>
          </div>

          {error && <p className="form-error">{error}</p>}
          {notice && <p className="settings-message">{notice}</p>}
        </section>

        <aside className="panel product-summary-card">
          <p className="eyebrow">Preview</p>
          <h2>{name || 'Новий товар'}</h2>
          <p>{description || 'Опис зʼявиться тут після заповнення форми.'}</p>

          <div className="summary-pill-list">
            <span>{selectedWarehouse ? getValue(selectedWarehouse, 'name', 'Name') : 'Склад не обрано'}</span>
            <span>{selectedCategories.length ? selectedCategories.map((item) => getValue(item, 'name', 'Name')).join(', ') : 'Без категорії'}</span>
            <span>Варіантів: {variants.length}</span>
            <span>Фото: {images.length}</span>
          </div>

          <div className="summary-line">
            <span>Загальна кількість</span>
            <strong>{summary.quantityTotal.toLocaleString('uk-UA')}</strong>
          </div>
          <div className="summary-line">
            <span>Собівартість залишку</span>
            <strong>{summary.costTotal.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-line">
            <span>Потенційний продаж</span>
            <strong>{summary.saleTotal.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-total">
            <span>Потенційний прибуток</span>
            <strong>{summary.profit.toLocaleString('uk-UA')} грн</strong>
          </div>

          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створення товару і завантаження фото...' : 'Створити товар'}
          </Button>
        </aside>
      </form>
    </section>
  )
}
