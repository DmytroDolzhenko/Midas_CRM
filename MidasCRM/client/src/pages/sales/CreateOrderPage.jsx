import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'

const serviceTypes = [
  { id: 0, label: 'Двері - двері' },
  { id: 1, label: 'Двері - склад' },
  { id: 2, label: 'Склад - склад' },
  { id: 3, label: 'Склад - двері' },
]

const cargoTypes = [
  { id: 1, label: 'Вантаж' },
  { id: 2, label: 'Документи' },
  { id: 3, label: 'Посилка' },
]

const paymentMethods = [
  { id: 0, label: 'Повна оплата' },
  { id: 1, label: 'Післяплата' },
  { id: 2, label: 'Оплачує відправник' },
]

export function CreateOrderPage({ customers, products, onBack, onCreate }) {
  const [productQuery, setProductQuery] = useState('')
  const [isProductPickerOpen, setIsProductPickerOpen] = useState(false)
  const [productId, setProductId] = useState(String(products[0]?.id ?? ''))
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [quantity, setQuantity] = useState(1)
  const [city, setCity] = useState('Київ')
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
      setError(submitError.message || 'Не вдалося створити продаж')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Sales</p>
          <h1>Новий продаж</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>До продажів</Button>
      </div>

      <form className="wide-form" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <div className="form-grid-3">
            <label className="field span-2">
              <span>Товар</span>
              <button className="product-picker-button" type="button" onClick={() => setIsProductPickerOpen(true)}>
                <strong>{selectedProduct?.name ?? 'Оберіть товар'}</strong>
                <small>{selectedProduct ? `${selectedProduct.sku} · ${selectedProduct.warehouse}` : 'Відкрити каталог товарів'}</small>
              </button>
            </label>
            <label className="field">
              <span>Клієнт</span>
              <select required value={selectedCustomerId} onChange={(event) => setCustomerId(event.target.value)}>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Кількість</span>
              <input min="1" type="number" value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} />
            </label>
          </div>

          <div className="delivery-box">
            <strong>Параметри замовлення</strong>
            <div className="form-grid-3">
              <label className="field">
                <span>Тип сервісу</span>
                <select value={serviceType} onChange={(event) => setServiceType(Number(event.target.value))}>
                  {serviceTypes.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Тип вантажу</span>
                <select value={cargoType} onChange={(event) => setCargoType(Number(event.target.value))}>
                  {cargoTypes.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Оплата</span>
                <select value={paymentMethod} onChange={(event) => setPaymentMethod(Number(event.target.value))}>
                  {paymentMethods.map((item) => (
                    <option key={item.id} value={item.id}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className="field span-2">
                <span>Опис</span>
                <textarea rows="3" value={description} onChange={(event) => setDescription(event.target.value)} placeholder={selectedProduct?.name ?? ''} />
              </label>
            </div>
          </div>

          <div className="delivery-box">
            <strong>Адреса доставки</strong>
            <div className="form-grid-3">
              <label className="field">
                <span>Місто</span>
                <input required maxLength="100" value={city} onChange={(event) => setCity(event.target.value)} />
              </label>
              <label className="field">
                <span>Поштовий код</span>
                <input min="1" type="number" value={postalCode} onChange={(event) => setPostalCode(Number(event.target.value))} />
              </label>
              <label className="field">
                <span>Відділення НП</span>
                <input min="1" type="number" value={postDepartmentNumber} onChange={(event) => setPostDepartmentNumber(Number(event.target.value))} />
              </label>
            </div>
          </div>

          {error && <p className="form-error">{error}</p>}
        </section>

        <section className="panel summary-panel">
          <h2>Підсумок продажу</h2>
          <div className="summary-line">
            <span>Товар</span>
            <strong>{selectedProduct?.name ?? '-'}</strong>
          </div>
          <div className="summary-line">
            <span>Кількість</span>
            <strong>{quantity}</strong>
          </div>
          <div className="summary-total">
            <span>Орієнтовна сума</span>
            <strong>{subtotal.toLocaleString('uk-UA')} грн</strong>
          </div>
          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створення...' : 'Створити продаж'}
          </Button>
        </section>
      </form>

      {isProductPickerOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsProductPickerOpen(false)}>
          <section className="product-picker-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Catalog</p>
                <h2>Оберіть товар</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setIsProductPickerOpen(false)}>x</button>
            </div>
            <div className="product-picker-search">
              <input value={productQuery} onChange={(event) => setProductQuery(event.target.value)} placeholder="Пошук за назвою, артикулом або складом" />
            </div>
            <div className="product-picker-list">
              {filteredProducts.map((product) => (
                <button key={product.id} type="button" onClick={() => chooseProduct(product)}>
                  <span>
                    <strong>{product.name}</strong>
                    <small>{product.sku} · {product.category} · {product.warehouse}</small>
                  </span>
                  <b>{product.price.toLocaleString('uk-UA')} грн</b>
                </button>
              ))}
            </div>
          </section>
        </div>
      )}
    </section>
  )
}

