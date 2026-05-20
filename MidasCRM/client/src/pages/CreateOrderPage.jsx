import { useMemo, useState } from 'react'
import { Button } from '../components/Button.jsx'

export function CreateOrderPage({ customers, products, onBack, onCreate }) {
  const [productQuery, setProductQuery] = useState('')
  const [productId, setProductId] = useState(String(products[0]?.id ?? ''))
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [quantity, setQuantity] = useState(1)
  const [city, setCity] = useState('Київ')
  const [postalCode, setPostalCode] = useState(1)
  const [postDepartmentNumber, setPostDepartmentNumber] = useState(1)
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const selectedProductId = productId || String(products[0]?.id ?? '')
  const selectedCustomerId = customerId || String(customers[0]?.id ?? '')

  const filteredProducts = useMemo(
    () =>
      products.filter((product) =>
        `${product.name} ${product.sku}`.toLowerCase().includes(productQuery.toLowerCase()),
      ),
    [productQuery, products],
  )
  const selectedProduct = products.find((product) => product.id === Number(selectedProductId))
  const subtotal = (selectedProduct?.price ?? 0) * quantity

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
        <Button variant="secondary" onClick={onBack}>
          До продажів
        </Button>
      </div>

      <form className="wide-form" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <div className="form-grid-3">
            <label className="field span-2">
              <span>Товар або артикул</span>
              <input
                list="product-options"
                value={productQuery}
                onChange={(event) => setProductQuery(event.target.value)}
                placeholder="Почни вводити назву або артикул"
              />
              <datalist id="product-options">
                {filteredProducts.map((product) => (
                  <option key={product.id} value={`${product.name} ${product.sku}`} />
                ))}
              </datalist>
            </label>
            <label className="field">
              <span>Обраний товар</span>
              <select required value={selectedProductId} onChange={(event) => setProductId(event.target.value)}>
                {filteredProducts.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.name} - {product.sku}
                  </option>
                ))}
              </select>
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
    </section>
  )
}
