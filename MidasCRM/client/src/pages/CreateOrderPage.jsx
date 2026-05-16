import { useMemo, useState } from 'react'
import { Button } from '../components/Button.jsx'

export function CreateOrderPage({ customers, products, onBack, onCreate }) {
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [productId, setProductId] = useState(String(products[0]?.id ?? ''))
  const [quantity, setQuantity] = useState(1)
  const [city, setCity] = useState('Київ')

  const selectedCustomer = customers.find((customer) => customer.id === Number(customerId))
  const selectedProduct = products.find((product) => product.id === Number(productId))
  const total = useMemo(() => (selectedProduct?.price ?? 0) * quantity, [quantity, selectedProduct])

  function handleSubmit(event) {
    event.preventDefault()
    onCreate({
      customer: selectedCustomer.name,
      product: selectedProduct.name,
      quantity,
      city,
      total,
    })
  }

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Create order</p>
          <h1>Нове замовлення</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>
          До списку
        </Button>
      </div>

      <form className="form-grid" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <h2>Дані замовлення</h2>
          <label className="field">
            <span>Клієнт</span>
            <select value={customerId} onChange={(event) => setCustomerId(event.target.value)}>
              {customers.map((customer) => (
                <option key={customer.id} value={customer.id}>
                  {customer.name}
                </option>
              ))}
            </select>
          </label>
          <label className="field">
            <span>Товар</span>
            <select value={productId} onChange={(event) => setProductId(event.target.value)}>
              {products.map((product) => (
                <option key={product.id} value={product.id}>
                  {product.name}
                </option>
              ))}
            </select>
          </label>
          <div className="field-row">
            <label className="field">
              <span>Кількість</span>
              <input
                min="1"
                type="number"
                value={quantity}
                onChange={(event) => setQuantity(Number(event.target.value))}
              />
            </label>
            <label className="field">
              <span>Місто</span>
              <input value={city} onChange={(event) => setCity(event.target.value)} />
            </label>
          </div>
        </section>

        <aside className="panel summary-panel">
          <h2>Підсумок</h2>
          <div className="summary-line">
            <span>Клієнт</span>
            <strong>{selectedCustomer?.name}</strong>
          </div>
          <div className="summary-line">
            <span>Товар</span>
            <strong>{selectedProduct?.name}</strong>
          </div>
          <div className="summary-total">
            <span>Разом</span>
            <strong>{total.toLocaleString('uk-UA')} грн</strong>
          </div>
          <Button className="full-width" type="submit">
            Створити
          </Button>
        </aside>
      </form>
    </section>
  )
}
