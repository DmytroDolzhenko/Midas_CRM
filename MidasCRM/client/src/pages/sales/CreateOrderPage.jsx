import { useMemo, useState } from 'react'
import { Button } from '../../components/Button.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Sales.module.css'

const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')

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
  const [isProductPickerOpen, setIsProductPickerOpen] = useState(false)
  const [isVariantPickerOpen, setIsVariantPickerOpen] = useState(false)
  const [productQuery, setProductQuery] = useState('')
  const [variantQuery, setVariantQuery] = useState('')
  const [activeProduct, setActiveProduct] = useState(null)
  const [selectedVariantIds, setSelectedVariantIds] = useState([])
  const [orderItems, setOrderItems] = useState([])
  const [isNewCustomer, setIsNewCustomer] = useState(false)
  const [customerQuery, setCustomerQuery] = useState('')
  const [customerId, setCustomerId] = useState(String(customers[0]?.id ?? ''))
  const [newCustomer, setNewCustomer] = useState({ name: '', surname: '', phone: '', email: '' })
  const [city, setCity] = useState('Київ')
  const [postDepartmentNumber, setPostDepartmentNumber] = useState(1)
  const [deliveryPointType, setDeliveryPointType] = useState('branch')
  const [serviceType, setServiceType] = useState(2)
  const [cargoType, setCargoType] = useState(1)
  const [paymentMethod, setPaymentMethod] = useState(1)
  const [description, setDescription] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const filteredProducts = useMemo(
    () => products.filter((product) =>
      `${product.name} ${product.category} ${product.warehouse}`.toLowerCase().includes(productQuery.toLowerCase())),
    [productQuery, products],
  )

  const filteredVariants = useMemo(
    () => (activeProduct?.variants ?? []).filter((variant) =>
      `${variant.uniqCode} ${variant.color} ${variant.size}`.toLowerCase().includes(variantQuery.toLowerCase())),
    [activeProduct, variantQuery],
  )

  const filteredCustomers = useMemo(
    () => customers.filter((customer) => customer.name.toLowerCase().includes(customerQuery.toLowerCase())),
    [customerQuery, customers],
  )

  const subtotal = useMemo(
    () => orderItems.reduce((sum, item) => sum + item.quantity * item.sellPrice, 0),
    [orderItems],
  )

  function openVariants(product) {
    setActiveProduct(product)
    setVariantQuery('')
    setSelectedVariantIds([])
    setIsVariantPickerOpen(true)
  }

  function toggleVariant(variantId) {
    setSelectedVariantIds((current) => current.includes(variantId)
      ? current.filter((id) => id !== variantId)
      : [...current, variantId])
  }

  function addVariantsToOrder() {
    const selectedVariants = (activeProduct?.variants ?? []).filter((variant) => selectedVariantIds.includes(variant.id))

    setOrderItems((currentItems) => {
      const nextItems = [...currentItems]
      selectedVariants.forEach((variant) => {
        if (nextItems.some((item) => item.productVariantId === variant.id)) {
          return
        }

        nextItems.push({
          productVariantId: variant.id,
          productName: activeProduct.name,
          variantLabel: `${variant.color} / ${variant.size}`,
          stockQuantity: variant.stockQuantity,
          sellPrice: variant.sellPrice,
          quantity: 1,
        })
      })

      return nextItems
    })

    setIsVariantPickerOpen(false)
    setSelectedVariantIds([])
  }

  function updateItemQuantity(productVariantId, quantity) {
    setOrderItems((currentItems) => currentItems.map((item) => {
      if (item.productVariantId !== productVariantId) {
        return item
      }

      const safeQuantity = Math.max(1, Math.min(Number(quantity) || 1, item.stockQuantity))
      return { ...item, quantity: safeQuantity }
    }))
  }

  function removeItem(productVariantId) {
    setOrderItems((currentItems) => currentItems.filter((item) => item.productVariantId !== productVariantId))
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')

    if (!orderItems.length) {
      setError('Додайте хоча б один товар до замовлення')
      return
    }

    if (!isNewCustomer && !customerId) {
      setError('Оберіть клієнта')
      return
    }

    if (isNewCustomer && (!newCustomer.name.trim() || !newCustomer.phone.trim() || !newCustomer.email.trim())) {
      setError('Заповніть обов?язкові поля нового замовника')
      return
    }

    setIsSubmitting(true)

    try {
      await onCreate({
        isNewCustomer,
        customerId,
        newCustomer,
        city,
        postDepartmentNumber,
        deliveryPointType,
        serviceType,
        cargoType,
        paymentMethods: paymentMethod,
        description: description.trim(),
        items: orderItems.map((item) => ({
          productVariantId: item.productVariantId,
          quantity: item.quantity,
        })),
      })
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося створити замовлення')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className={cx('page-stack')}>
      <div className={cx('page-header')}>
        <div>
          <p className={cx('eyebrow')}>Sales</p>
          <h1>Створення замовлення</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>До продажів</Button>
      </div>

      <form className={cx('wide-form')} onSubmit={handleSubmit}>
        <section className={cx('panel', 'form-section')}>
          <div className={cx('form-grid-3')}>
            <label className={cx('field', 'span-2')}>
              <span>Товари</span>
              <button className={cx('product-picker-button')} type="button" onClick={() => setIsProductPickerOpen(true)}>
                <strong>Відкрити асортимент</strong>
                <small>Оберіть Product, потім ProductVariant і додайте в замовлення</small>
              </button>
            </label>
          </div>

          <div className={cx('delivery-box')}>
            <strong>Товари в замовленні</strong>
            {!orderItems.length ? <p>Ще не додано жодного товару.</p> : (
              <div className={cx('order-items-grid')}>
                {orderItems.map((item) => (
                  <div className={cx('order-item-row')} key={item.productVariantId}>
                    <div>
                      <strong>{item.productName}</strong>
                      <small>{item.variantLabel}</small>
                    </div>
                    <label className={cx('field')}>
                      <span>Кількість (макс. {item.stockQuantity})</span>
                      <input
                        min="1"
                        max={item.stockQuantity}
                        type="number"
                        value={item.quantity}
                        onChange={(event) => updateItemQuantity(item.productVariantId, event.target.value)}
                      />
                    </label>
                    <strong>{(item.sellPrice * item.quantity).toLocaleString('uk-UA')} грн</strong>
                    <button type="button" className={cx('secondary-button')} onClick={() => removeItem(item.productVariantId)}>Видалити</button>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className={cx('delivery-box')}>
            <strong>Замовник</strong>
            <label className={cx('inline-switch')}>
              <input type="checkbox" checked={isNewCustomer} onChange={(event) => setIsNewCustomer(event.target.checked)} />
              <span>{isNewCustomer ? 'Новий замовник' : 'Існуючий замовник'}</span>
            </label>

            {!isNewCustomer ? (
              <div className={cx('form-grid-3')}>
                <label className={cx('field', 'span-2')}>
                  <span>Пошук клієнта</span>
                  <input value={customerQuery} onChange={(event) => setCustomerQuery(event.target.value)} placeholder="Введіть ім?я клієнта" />
                </label>
                <label className={cx('field')}>
                  <span>Клієнт</span>
                  <select value={customerId} onChange={(event) => setCustomerId(event.target.value)}>
                    {filteredCustomers.map((customer) => (
                      <option key={customer.id} value={customer.id}>{customer.name}</option>
                    ))}
                  </select>
                </label>
              </div>
            ) : (
              <div className={cx('form-grid-3')}>
                <label className={cx('field')}><span>Ім?я</span><input value={newCustomer.name} onChange={(event) => setNewCustomer((s) => ({ ...s, name: event.target.value }))} /></label>
                <label className={cx('field')}><span>Прізвище</span><input value={newCustomer.surname} onChange={(event) => setNewCustomer((s) => ({ ...s, surname: event.target.value }))} /></label>
                <label className={cx('field')}><span>Телефон</span><input value={newCustomer.phone} onChange={(event) => setNewCustomer((s) => ({ ...s, phone: event.target.value }))} /></label>
                <label className={cx('field', 'span-2')}><span>Email</span><input type="email" value={newCustomer.email} onChange={(event) => setNewCustomer((s) => ({ ...s, email: event.target.value }))} /></label>
              </div>
            )}
          </div>

          <div className={cx('delivery-box')}>
            <strong>Параметри замовлення</strong>
            <div className={cx('form-grid-3')}>
              <label className={cx('field')}>
                <span>Тип сервісу</span>
                <select value={serviceType} onChange={(event) => setServiceType(Number(event.target.value))}>
                  {serviceTypes.map((item) => <option key={item.id} value={item.id}>{item.label}</option>)}
                </select>
              </label>
              <label className={cx('field')}>
                <span>Тип вантажу</span>
                <select value={cargoType} onChange={(event) => setCargoType(Number(event.target.value))}>
                  {cargoTypes.map((item) => <option key={item.id} value={item.id}>{item.label}</option>)}
                </select>
              </label>
              <label className={cx('field')}>
                <span>Оплата</span>
                <select value={paymentMethod} onChange={(event) => setPaymentMethod(Number(event.target.value))}>
                  {paymentMethods.map((item) => <option key={item.id} value={item.id}>{item.label}</option>)}
                </select>
              </label>
              <label className={cx('field', 'span-2')}>
                <span>Опис</span>
                <textarea rows="3" value={description} onChange={(event) => setDescription(event.target.value)} />
              </label>
            </div>
          </div>

          <div className={cx('delivery-box')}>
            <strong>Адреса доставки</strong>
            <div className={cx('form-grid-3')}>
              <label className={cx('field')}><span>Місто</span><input required value={city} onChange={(event) => setCity(event.target.value)} /></label>
              <label className={cx('field')}><span>Номер точки НП</span><input min="1" type="number" value={postDepartmentNumber} onChange={(event) => setPostDepartmentNumber(Number(event.target.value))} /></label>
              <label className={cx('field')}>
                <span>Тип доставки</span>
                <select value={deliveryPointType} onChange={(event) => setDeliveryPointType(event.target.value)}>
                  <option value="branch">Відділення</option>
                  <option value="parcelLocker">Поштомат</option>
                </select>
              </label>
            </div>
          </div>

          {error && <p className={cx('form-error')}>{error}</p>}
        </section>

        <section className={cx('panel', 'summary-panel')}>
          <h2>Підсумок замовлення</h2>
          <div className={cx('summary-line')}><span>Позицій</span><strong>{orderItems.length}</strong></div>
          <div className={cx('summary-total')}><span>Орієнтовна сума</span><strong>{subtotal.toLocaleString('uk-UA')} грн</strong></div>
          <Button className={cx('full-width')} type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Створення...' : 'Створити замовлення'}
          </Button>
        </section>
      </form>

      {isProductPickerOpen && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={() => setIsProductPickerOpen(false)}>
          <section className={cx('product-picker-modal')} role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div><p className={cx('eyebrow')}>Catalog</p><h2>Оберіть Product</h2></div>
              <button className={cx('modal-close-button')} type="button" onClick={() => setIsProductPickerOpen(false)}>x</button>
            </div>
            <div className={cx('product-picker-search')}><input value={productQuery} onChange={(event) => setProductQuery(event.target.value)} placeholder="Пошук товару" /></div>
            <div className={cx('product-picker-list')}>
              {filteredProducts.map((product) => (
                <button key={product.id} type="button" onClick={() => { setIsProductPickerOpen(false); openVariants(product) }}>
                  <span><strong>{product.name}</strong><small>{product.category} · {product.warehouse}</small></span>
                  <b>{product.stock} од.</b>
                </button>
              ))}
            </div>
          </section>
        </div>
      )}

      {isVariantPickerOpen && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={() => setIsVariantPickerOpen(false)}>
          <section className={cx('product-picker-modal')} role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div><p className={cx('eyebrow')}>Variants</p><h2>{activeProduct?.name}</h2></div>
              <button className={cx('modal-close-button')} type="button" onClick={() => setIsVariantPickerOpen(false)}>x</button>
            </div>
            <div className={cx('product-picker-search')}><input value={variantQuery} onChange={(event) => setVariantQuery(event.target.value)} placeholder="Пошук варіанту" /></div>
            <div className={cx('product-picker-list')}>
              {filteredVariants.map((variant) => (
                <label key={variant.id} className={cx('variant-picker-row')}>
                  <input type="checkbox" checked={selectedVariantIds.includes(variant.id)} onChange={() => toggleVariant(variant.id)} />
                  <span>
                    <strong>{variant.uniqCode}</strong>
                    <small>{variant.color} / {variant.size} · доступно {variant.stockQuantity}</small>
                  </span>
                  <b>{variant.sellPrice.toLocaleString('uk-UA')} грн</b>
                </label>
              ))}
            </div>
            <div className={cx('settings-actions')}>
              <Button type="button" onClick={addVariantsToOrder} disabled={!selectedVariantIds.length}>Додати до замовлення</Button>
            </div>
          </section>
        </div>
      )}
    </section>
  )
}

