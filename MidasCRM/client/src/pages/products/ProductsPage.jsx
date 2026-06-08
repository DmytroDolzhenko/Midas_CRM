import { useMemo, useRef, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Products.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



const PAGE_SIZE = 8
const stockFilters = [
  { id: 'all', label: 'всі' },
  { id: 'available', label: 'в наявності' },
  { id: 'empty', label: 'немає в наявності' },
]

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function ExportIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M12 3v12" />
      <path d="m7 10 5 5 5-5" />
      <path d="M5 21h14" />
    </svg>
  )
}

export function ProductsPage({ products, warehouses = [], onNavigate, onCreateWarehouse, onUpdateWarehouse }) {
  const firstWarehouseId = String(getValue(warehouses[0], 'id', 'Id') ?? '')
  const [activeWarehouseId, setActiveWarehouseId] = useState(firstWarehouseId)
  const [search, setSearch] = useState('')
  const [stockFilter, setStockFilter] = useState('all')
  const [isFilterOpen, setIsFilterOpen] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null)
  const [editingWarehouse, setEditingWarehouse] = useState(null)
  const [warehouseName, setWarehouseName] = useState('')
  const [warehouseError, setWarehouseError] = useState('')
  const [importFileName, setImportFileName] = useState('')
  const [minCost, setMinCost] = useState('')
  const [maxCost, setMaxCost] = useState('')
  const [minPrice, setMinPrice] = useState('')
  const [maxPrice, setMaxPrice] = useState('')
  const [page, setPage] = useState(1)
  const fileInputRef = useRef(null)

  const selectedWarehouseId = activeWarehouseId || firstWarehouseId
  const activeWarehouse = warehouses.find((item) => String(getValue(item, 'id', 'Id')) === String(selectedWarehouseId))

  const warehouseProducts = useMemo(
    () => products.filter((product) => String(product.warehouseId) === String(selectedWarehouseId)),
    [products, selectedWarehouseId],
  )

  const filteredProducts = useMemo(
    () =>
      warehouseProducts.filter((product) => {
        const matchesSearch = `${product.name} ${product.sku} ${product.category ?? ''}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesStock =
          stockFilter === 'all' ||
          (stockFilter === 'available' && product.stock > 0) ||
          (stockFilter === 'empty' && product.stock <= 0)
        const matchesCost =
          (!minCost || Number(product.cost) >= Number(minCost)) &&
          (!maxCost || Number(product.cost) <= Number(maxCost))
        const matchesPrice =
          (!minPrice || Number(product.price) >= Number(minPrice)) &&
          (!maxPrice || Number(product.price) <= Number(maxPrice))

        return matchesSearch && matchesStock && matchesCost && matchesPrice
      }),
    [maxCost, maxPrice, minCost, minPrice, search, stockFilter, warehouseProducts],
  )

  const paginatedProducts = filteredProducts.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const totals = useMemo(() => {
    const available = warehouseProducts.filter((product) => product.stock > 0)
    const stockUnits = warehouseProducts.reduce((sum, product) => sum + Number(product.stock), 0)
    const storedValue = warehouseProducts.reduce((sum, product) => sum + product.stock * product.cost, 0)
    const possibleIncome = warehouseProducts.reduce((sum, product) => sum + product.stock * (product.price - product.cost), 0)

    return {
      total: warehouseProducts.length,
      available: available.length,
      stockUnits,
      storedValue,
      possibleIncome,
    }
  }, [warehouseProducts])

  function updateWarehouse(value) {
    setActiveWarehouseId(String(value))
    setPage(1)
  }

  function updateSearch(value) {
    setSearch(value)
    setPage(1)
  }

  function updateStockFilter(value) {
    setStockFilter(value)
    setPage(1)
  }

  function openCreateWarehouse() {
    setEditingWarehouse({ mode: 'create' })
    setWarehouseName('')
    setWarehouseError('')
  }

  function openEditWarehouse() {
    if (!activeWarehouse) {
      return
    }

    setEditingWarehouse({ mode: 'edit', id: getValue(activeWarehouse, 'id', 'Id') })
    setWarehouseName(getValue(activeWarehouse, 'name', 'Name') ?? '')
    setWarehouseError('')
  }

  async function submitWarehouse(event) {
    event.preventDefault()

    if (!warehouseName.trim()) {
      setWarehouseError('Вкажіть назву складу')
      return
    }

    try {
      if (editingWarehouse?.mode === 'edit') {
        await onUpdateWarehouse(editingWarehouse.id, { name: warehouseName.trim() })
      } else {
        await onCreateWarehouse({ name: warehouseName.trim() })
      }
      setEditingWarehouse(null)
      setWarehouseName('')
    } catch (error) {
      setWarehouseError(error.message || 'Не вдалося зберегти склад')
    }
  }

  function exportProducts() {
    const header = ['Артикул', 'Назва', 'Категорія', 'Доступно', 'Собівартість', 'Ціна продажу']
    const rows = filteredProducts.map((product) => [
      product.sku,
      product.name,
      product.category,
      product.stock,
      product.cost,
      product.price,
    ])
    const csv = [header, ...rows].map((row) => row.join(';')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')

    link.href = url
    link.download = 'midas-products.csv'
    link.click()
    URL.revokeObjectURL(url)
  }

  function handleImport(event) {
    const file = event.target.files?.[0]

    if (file) {
      setImportFileName(file.name)
    }
  }

  return (
    
    <section className={cx('catalog-page')}>
      <div className={cx('catalog-toolbar', 'panel')}>
        <div className={cx('warehouse-tabs')}>
          {warehouses.map((warehouse) => {
            const warehouseId = String(getValue(warehouse, 'id', 'Id'))
            return (
              <button
                key={warehouseId}
                type="button"
                className={warehouseId === String(selectedWarehouseId) ? cx('warehouse-tab', 'active') : cx('warehouse-tab')}
                onClick={() => updateWarehouse(warehouseId)}
              >
                {getValue(warehouse, 'name', 'Name')}
              </button>
            )
          })}
          <button className={cx('warehouse-add-button')} type="button" onClick={openCreateWarehouse}>+</button>
          <button className={cx('dots-button')} type="button" onClick={openEditWarehouse}>...</button>
        </div>

        <div className={cx('catalog-filters')}>
          <input
            aria-label="Пошук товарів"
            placeholder="Пошук товарів"
            value={search}
            onChange={(event) => updateSearch(event.target.value)}
          />
          <div className={cx('radio-group')}>
            {stockFilters.map((filter) => (
              <label key={filter.id}>
                <input
                  type="radio"
                  name="stock-filter"
                  checked={stockFilter === filter.id}
                  onChange={() => updateStockFilter(filter.id)}
                />
                {filter.label}
              </label>
            ))}
          </div>
          <button className={cx('icon-button', 'flat-icon-button')} type="button" aria-label="Експорт" onClick={exportProducts}>
            <ExportIcon />
          </button>
          <button className={cx('secondary-button')} type="button" onClick={() => setIsFilterOpen((isOpen) => !isOpen)}>
            Фільтр
          </button>
        </div>

        {isFilterOpen && (
          <div className={cx('inline-filter-panel')}>
            <label className={cx('field')}>
              <span>Собівартість від</span>
              <input min="0" type="number" value={minCost} onChange={(event) => { setMinCost(event.target.value); setPage(1) }} />
            </label>
            <label className={cx('field')}>
              <span>Собівартість до</span>
              <input min="0" type="number" value={maxCost} onChange={(event) => { setMaxCost(event.target.value); setPage(1) }} />
            </label>
            <label className={cx('field')}>
              <span>Ціна від</span>
              <input min="0" type="number" value={minPrice} onChange={(event) => { setMinPrice(event.target.value); setPage(1) }} />
            </label>
            <label className={cx('field')}>
              <span>Ціна до</span>
              <input min="0" type="number" value={maxPrice} onChange={(event) => { setMaxPrice(event.target.value); setPage(1) }} />
            </label>
          </div>
        )}
      </div>

      <div className={cx('inventory-widget-grid')}>
        <article><span>Усього товару</span><strong>{totals.total}</strong></article>
        <article><span>В наявності</span><strong>{totals.available} товарів</strong></article>
        <article><span>Доступно</span><strong>{totals.stockUnits} одиниць</strong></article>
        <article><span>Товарів на складі</span><strong>{totals.storedValue.toLocaleString('uk-UA')} грн</strong></article>
        <article><span>Можливий дохід</span><strong>{totals.possibleIncome.toLocaleString('uk-UA')} грн</strong></article>
      </div>

      <div className={cx('inventory-actions', 'panel')}>
        <button className={cx('success-button')} type="button" onClick={() => onNavigate('createProduct')}>
          Додати товар
        </button>
        <button className={cx('primary-button')} type="button" onClick={() => fileInputRef.current?.click()}>
          Імпортувати
        </button>
        <input ref={fileInputRef} className={cx('hidden-file-input')} type="file" accept=".csv,.xlsx" onChange={handleImport} />
        {importFileName && <span className={cx('import-file-name')}>Імпортовано: {importFileName}</span>}
      </div>

      <section className={cx('products-table-card', 'panel')}>
        <div className={cx('products-layout-header')}>
          <span><input type="checkbox" /></span>
          <span>Товар</span>
          <span>Доступно</span>
          <span>Продажі</span>
          <span>Собів.</span>
          <span>Ціна продажу</span>
          <span>Націнка</span>
        </div>

        {paginatedProducts.map((product) => {
          const markup = product.cost > 0 ? Math.round(((product.price - product.cost) / product.cost) * 100) : 0

          return (
            <div className={cx('product-layout-row')} key={product.id}>
              <label className={cx('product-check')}><input type="checkbox" /></label>
              <div
                className={cx('product-main-cell')}
                role="button"
                tabIndex={0}
                onClick={() => setEditingProduct(product)}
                onKeyDown={(event) => { if (event.key === 'Enter') setEditingProduct(product) }}
              >
                <div className={cx('product-preview')}>
                  {product.imageUrl ? <img alt={product.name} src={product.imageUrl} /> : <span>{product.name?.slice(0, 1)?.toUpperCase() || 'P'}</span>}
                </div>
                <div>
                  <strong>{product.name}</strong>
                  <span>ID: {product.sku}</span>
                  <span>Категорія: {product.category || 'Без категорії'}</span>
                  <button type="button" onClick={(event) => { event.stopPropagation(); setEditingProduct(product) }}>Варіанти</button>
                </div>
              </div>
              <div className={cx('large-product-cell', 'available-cell')}>
                <strong>{product.stock}</strong>
                <span>{product.unit}</span>
              </div>
              <div className={cx('large-product-cell')}>
                <strong>0</strong>
                <span>продажів</span>
              </div>
              <div>{product.cost.toLocaleString('uk-UA')} грн</div>
              <div>{product.price.toLocaleString('uk-UA')} грн</div>
              <div>{markup}%</div>
            </div>
          )
        })}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredProducts.length} onPageChange={setPage} />
      </section>

      {editingProduct && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={() => setEditingProduct(null)}>
          <section className={cx('small-modal')} role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div>
                <p className={cx('eyebrow')}>Product</p>
                <h2>Редагування товару</h2>
              </div>
              <button className={cx('modal-close-button')} type="button" onClick={() => setEditingProduct(null)}>x</button>
            </div>
            <div className={cx('settings-grid')}>
              <label className={cx('field')}>
                <span>Назва</span>
                <input readOnly value={editingProduct.name} />
              </label>
              <label className={cx('field')}>
                <span>Артикул</span>
                <input readOnly value={editingProduct.sku} />
              </label>
              <label className={cx('field')}>
                <span>Склад</span>
                <input readOnly value={editingProduct.warehouse} />
              </label>
              <label className={cx('field')}>
                <span>Категорія</span>
                <input readOnly value={editingProduct.category || 'Без категорії'} />
              </label>
              <label className={cx('field')}>
                <span>Ціна продажу</span>
                <input readOnly value={`${editingProduct.price} грн`} />
              </label>
            </div>
            <div className={cx('variant-list')}>
              {(editingProduct.variants ?? []).map((variant) => (
                <article key={variant.id}>
                  <strong>{variant.uniqCode || `Variant #${variant.id}`}</strong>
                  <span>{variant.color || '-'} / {variant.size || '-'}</span>
                  <span>Доступно: {variant.stockQuantity} з {variant.originalStockQuantity}</span>
                  <span>У замовленнях: {variant.reservedQuantity}</span>
                  <span>{variant.sellPrice.toLocaleString('uk-UA')} грн</span>
                </article>
              ))}
              {!editingProduct.variants?.length && <p>Варіантів для цього товару немає.</p>}
            </div>
            <div className={cx('settings-actions')}>
              <button className={cx('primary-button')} type="button" onClick={() => setEditingProduct(null)}>Готово</button>
            </div>
          </section>
        </div>
      )}

      {editingWarehouse && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={() => setEditingWarehouse(null)}>
          <form className={cx('small-modal')} role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()} onSubmit={submitWarehouse}>
            <div className={cx('settings-header')}>
              <div>
                <p className={cx('eyebrow')}>Warehouse</p>
                <h2>{editingWarehouse.mode === 'edit' ? 'Редагувати склад' : 'Новий склад'}</h2>
              </div>
              <button className={cx('modal-close-button')} type="button" onClick={() => setEditingWarehouse(null)}>x</button>
            </div>
            <div className={cx('settings-grid')}>
              <label className={cx('field', 'span-2')}>
                <span>Назва складу</span>
                <input value={warehouseName} maxLength="100" onChange={(event) => setWarehouseName(event.target.value)} />
              </label>
            </div>
            {warehouseError && <p className={cx('settings-message')}>{warehouseError}</p>}
            <div className={cx('settings-actions')}>
              <button className={cx('secondary-button')} type="button" onClick={() => setEditingWarehouse(null)}>Скасувати</button>
              <button className={cx('primary-button')} type="submit">Зберегти</button>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}


