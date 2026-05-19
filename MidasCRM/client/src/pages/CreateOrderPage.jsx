import { useMemo, useState } from 'react'
import { Button } from '../components/Button.jsx'

const operationTypes = [
  { id: 'sale', label: 'Продаж' },
  { id: 'delivery', label: 'Доставка' },
  { id: 'reserve', label: 'Резерв' },
]

const accounts = ['Наложка NovaPay (23918...)', 'Monobank ФОП', 'Готівка', 'Безготівковий рахунок']
const channels = ['OLX-Наложка', 'Instagram', 'Prom.ua', 'Telegram', 'Сайт']

function getToday() {
  return new Date().toISOString().slice(0, 10)
}

export function CreateOrderPage({ customers, products, onBack, onCreate }) {
  const [operationType, setOperationType] = useState('sale')
  const [productQuery, setProductQuery] = useState('')
  const [productId, setProductId] = useState(String(products[0]?.id ?? ''))
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [quantity, setQuantity] = useState(1)
  const [discount, setDiscount] = useState(0)
  const [discountType, setDiscountType] = useState('percent')
  const [account, setAccount] = useState(accounts[0])
  const [selectedChannels, setSelectedChannels] = useState(['OLX-Наложка'])
  const [date, setDate] = useState(getToday())
  const [comment, setComment] = useState('')
  const [saleNumber, setSaleNumber] = useState('')
  const [expense, setExpense] = useState(0)
  const [deliveryMode, setDeliveryMode] = useState('simple')
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
        `${product.name} ${product.sku} ${product.brand}`.toLowerCase().includes(productQuery.toLowerCase()),
      ),
    [productQuery, products],
  )
  const selectedProduct = products.find((product) => product.id === Number(selectedProductId))
  const isDeliveryRelated =
    operationType === 'delivery' || selectedChannels.some((channel) => channel.toLowerCase().includes('наложка'))
  const subtotal = (selectedProduct?.price ?? 0) * quantity
  const discountAmount = discountType === 'percent' ? subtotal * (discount / 100) : discount
  const total = Math.max(subtotal - discountAmount, 0)
  const cost = (selectedProduct?.cost ?? 0) * quantity
  const profit = total - cost - expense

  function toggleChannel(channel) {
    setSelectedChannels((currentChannels) =>
      currentChannels.includes(channel)
        ? currentChannels.filter((item) => item !== channel)
        : [...currentChannels, channel],
    )
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await onCreate({
        code: saleNumber,
        productId: selectedProductId,
        customerId: selectedCustomerId,
        quantity,
        total,
        cost,
        profit,
        expense,
        operationType,
        account,
        channel: selectedChannels.join(', '),
        date,
        comment,
        deliveryMode: isDeliveryRelated ? deliveryMode : 'simple',
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
          <div className="tabs">
            {operationTypes.map((item) => (
              <button
                key={item.id}
                type="button"
                className={item.id === operationType ? 'tab-button active' : 'tab-button'}
                onClick={() => setOperationType(item.id)}
              >
                {item.label}
              </button>
            ))}
          </div>

          <div className="form-grid-3">
            <label className="field span-2">
              <span>Товар/артикул/бренд</span>
              <input
                list="product-options"
                value={productQuery}
                onChange={(event) => setProductQuery(event.target.value)}
                placeholder="Почни вводити назву, артикул або бренд"
              />
              <datalist id="product-options">
                {filteredProducts.map((product) => (
                  <option key={product.id} value={`${product.name} ${product.sku} ${product.brand}`} />
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
              <span>Оберіть клієнта</span>
              <select required value={selectedCustomerId} onChange={(event) => setCustomerId(event.target.value)}>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Рахунок</span>
              <select value={account} onChange={(event) => setAccount(event.target.value)}>
                {accounts.map((item) => (
                  <option key={item}>{item}</option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Дата</span>
              <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
            </label>
            <label className="field">
              <span>Кількість</span>
              <input min="1" type="number" value={quantity} onChange={(event) => setQuantity(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Знижка</span>
              <input min="0" type="number" value={discount} onChange={(event) => setDiscount(Number(event.target.value))} />
            </label>
            <label className="field">
              <span>Тип знижки</span>
              <select value={discountType} onChange={(event) => setDiscountType(event.target.value)}>
                <option value="percent">%</option>
                <option value="fixed">Фіксована сума</option>
              </select>
            </label>
            <label className="field">
              <span>Номер продажу</span>
              <input value={saleNumber} onChange={(event) => setSaleNumber(event.target.value)} />
            </label>
          </div>

          <div className="tag-selector">
            <span>Канал продажів/Теги</span>
            <div>
              {channels.map((channel) => (
                <button
                  key={channel}
                  type="button"
                  className={selectedChannels.includes(channel) ? 'tag active' : 'tag'}
                  onClick={() => toggleChannel(channel)}
                >
                  {channel}
                </button>
              ))}
            </div>
          </div>

          {isDeliveryRelated && (
            <div className="delivery-box">
              <strong>Логіка доставки</strong>
              <div className="tabs">
                <button
                  type="button"
                  className={deliveryMode === 'simple' ? 'tab-button active' : 'tab-button'}
                  onClick={() => setDeliveryMode('simple')}
                >
                  Простий продаж
                </button>
                <button
                  type="button"
                  className={deliveryMode === 'nova-post' ? 'tab-button active' : 'tab-button'}
                  onClick={() => setDeliveryMode('nova-post')}
                >
                  Продаж за ТТН Нової Пошти
                </button>
              </div>
              <div className="form-grid-3">
                <label className="field">
                  <span>Місто</span>
                  <input value={city} onChange={(event) => setCity(event.target.value)} />
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
          )}

          <label className="field">
            <span>Коментар</span>
            <textarea rows="4" value={comment} onChange={(event) => setComment(event.target.value)} />
          </label>

          <button className="expense-button" type="button" onClick={() => setExpense((currentExpense) => currentExpense + 100)}>
            + Додати витрату
          </button>

          {error && <p className="form-error">{error}</p>}
        </section>

        <section className="panel summary-panel">
          <h2>Підсумок продажу</h2>
          <div className="summary-line">
            <span>Підсумок</span>
            <strong>{subtotal.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-line">
            <span>Знижка</span>
            <strong>{discountAmount.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-line">
            <span>Витрати</span>
            <strong>{expense.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-total">
            <span>До оплати</span>
            <strong>{total.toLocaleString('uk-UA')} грн</strong>
          </div>
          <div className="summary-line">
            <span>Валовий прибуток</span>
            <strong>{profit.toLocaleString('uk-UA')} грн</strong>
          </div>
          <Button className="full-width" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створення...' : 'Створити продаж'}
          </Button>
        </section>
      </form>
    </section>
  )
}
