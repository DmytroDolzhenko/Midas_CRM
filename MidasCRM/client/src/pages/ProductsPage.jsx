import { useMemo, useRef, useState } from 'react'
import { Pagination } from '../components/Pagination.jsx'

const warehouses = ['Gorpcore', 'Основний склад', 'Шоурум Київ']
const PAGE_SIZE = 8
const stockFilters = [
  { id: 'all', label: 'всі' },
  { id: 'available', label: 'в наявності' },
  { id: 'empty', label: 'нема в наявності' },
]

function ExportIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M12 3v12" />
      <path d="m7 10 5 5 5-5" />
      <path d="M5 21h14" />
    </svg>
  )
}

function SettingsMiniIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <circle cx="12" cy="12" r="3" />
      <path d="M19.4 15a7.8 7.8 0 0 0 .1-2l2-1.5-2-3.5-2.4 1a8 8 0 0 0-1.7-1L15 5h-4l-.4 3a8 8 0 0 0-1.7 1l-2.4-1-2 3.5 2 1.5a7.8 7.8 0 0 0 .1 2l-2 1.5 2 3.5 2.4-1a8 8 0 0 0 1.7 1l.4 3h4l.4-3a8 8 0 0 0 1.7-1l2.4 1 2-3.5-2.2-1.5Z" />
    </svg>
  )
}

export function ProductsPage({ products, onNavigate }) {
  const [warehouse, setWarehouse] = useState('Gorpcore')
  const [warehouseList, setWarehouseList] = useState(warehouses)
  const [search, setSearch] = useState('')
  const [stockFilter, setStockFilter] = useState('all')
  const [isFilterOpen, setIsFilterOpen] = useState(false)
  const [isSettingsOpen, setIsSettingsOpen] = useState(false)
  const [isWarehouseModalOpen, setIsWarehouseModalOpen] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null)
  const [importFileName, setImportFileName] = useState('')
  const [newWarehouseName, setNewWarehouseName] = useState('')
  const [minCost, setMinCost] = useState('')
  const [maxCost, setMaxCost] = useState('')
  const [minPrice, setMinPrice] = useState('')
  const [maxPrice, setMaxPrice] = useState('')
  const [page, setPage] = useState(1)
  const fileInputRef = useRef(null)

  const visibleWarehouses = useMemo(
    () => Array.from(new Set([...warehouseList, ...products.map((product) => product.warehouse).filter(Boolean)])),
    [products, warehouseList],
  )

  const filteredProducts = useMemo(
    () =>
      products.filter((product) => {
        const matchesSearch = `${product.name} ${product.sku} ${product.brand} ${product.category}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesStock =
          stockFilter === 'all' ||
          (stockFilter === 'available' && product.stock > 0) ||
          (stockFilter === 'empty' && product.stock <= 0)
        const matchesWarehouse = product.warehouse === warehouse
        const matchesCost =
          (!minCost || Number(product.cost) >= Number(minCost)) &&
          (!maxCost || Number(product.cost) <= Number(maxCost))
        const matchesPrice =
          (!minPrice || Number(product.price) >= Number(minPrice)) &&
          (!maxPrice || Number(product.price) <= Number(maxPrice))

        return matchesSearch && matchesStock && matchesWarehouse && matchesCost && matchesPrice
      }),
    [maxCost, maxPrice, minCost, minPrice, products, search, stockFilter, warehouse],
  )

  const paginatedProducts = filteredProducts.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const totals = useMemo(() => {
    const warehouseProducts = products.filter((product) => product.warehouse === warehouse)
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
  }, [products, warehouse])

  function updateSearch(value) {
    setSearch(value)
    setPage(1)
  }

  function updateStockFilter(value) {
    setStockFilter(value)
    setPage(1)
  }

  function updateWarehouse(value) {
    setWarehouse(value)
    setPage(1)
  }

  function addWarehouse() {
    setNewWarehouseName('')
    setIsWarehouseModalOpen(true)
  }

  function saveWarehouse(event) {
    event.preventDefault()
    const nextWarehouseName = newWarehouseName.trim()

    if (!nextWarehouseName) {
      return
    }

    setWarehouseList((currentWarehouses) => [...currentWarehouses, nextWarehouseName])
    updateWarehouse(nextWarehouseName)
    setIsWarehouseModalOpen(false)
  }

  function renameWarehouse(index, name) {
    const nextName = name.trimStart()

    setWarehouseList((currentWarehouses) =>
      currentWarehouses.map((item, itemIndex) => (itemIndex === index ? nextName : item)),
    )

    if (warehouseList[index] === warehouse) {
      updateWarehouse(nextName)
    }
  }

  function exportProducts() {
    const header = ['Артикул', 'Назва', 'Бренд', 'Категорія', 'Доступно', 'Собівартість', 'Ціна продажу']
    const rows = filteredProducts.map((product) => [
      product.sku,
      product.name,
      product.brand,
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
    <section className="catalog-page">
      <div className="catalog-toolbar panel">
        <div className="warehouse-tabs">
          {visibleWarehouses.map((item) => (
            <button
              key={item}
              type="button"
              className={warehouse === item ? 'warehouse-tab active' : 'warehouse-tab'}
              onClick={() => updateWarehouse(item)}
            >
              {item}
            </button>
          ))}
          <button className="warehouse-add-button" type="button" onClick={addWarehouse}>+</button>
        </div>

        <div className="catalog-filters">
          <input
            aria-label="Пошукова фраза"
            placeholder="Пошукова фраза"
            value={search}
            onChange={(event) => updateSearch(event.target.value)}
          />
          <div className="radio-group">
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
          <button className="icon-button flat-icon-button" type="button" aria-label="Експорт" onClick={exportProducts}>
            <ExportIcon />
          </button>
          <button className="secondary-button" type="button" onClick={() => setIsFilterOpen((isOpen) => !isOpen)}>
            Фільтр
          </button>
        </div>

        {isFilterOpen && (
          <div className="inline-filter-panel">
            <label className="field">
              <span>Поточний склад</span>
              <select value={warehouse} onChange={(event) => updateWarehouse(event.target.value)}>
                {visibleWarehouses.map((item) => (
                  <option key={item}>{item}</option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>Пошук</span>
              <input value={search} onChange={(event) => updateSearch(event.target.value)} />
            </label>
            <label className="field">
              <span>Собівартість від</span>
              <input min="0" type="number" value={minCost} onChange={(event) => { setMinCost(event.target.value); setPage(1) }} />
            </label>
            <label className="field">
              <span>Собівартість до</span>
              <input min="0" type="number" value={maxCost} onChange={(event) => { setMaxCost(event.target.value); setPage(1) }} />
            </label>
            <label className="field">
              <span>Ціна від</span>
              <input min="0" type="number" value={minPrice} onChange={(event) => { setMinPrice(event.target.value); setPage(1) }} />
            </label>
            <label className="field">
              <span>Ціна до</span>
              <input min="0" type="number" value={maxPrice} onChange={(event) => { setMaxPrice(event.target.value); setPage(1) }} />
            </label>
          </div>
        )}
      </div>

      <div className="inventory-widget-grid">
        <article><span>УСЬОГО ТОВАРУ</span><strong>{totals.total}</strong></article>
        <article><span>В НАЯВНОСТІ</span><strong>{totals.available} товарів</strong></article>
        <article><span>ДОСТУПНО</span><strong>{totals.stockUnits.toFixed(2)} одиниць</strong></article>
        <article><span>ЗБЕРІГАЄТЬСЯ ТОВАРІВ НА</span><strong>{totals.storedValue.toLocaleString('uk-UA')} грн.</strong></article>
        <article><span>МОЖЛИВИЙ ДОХІД</span><strong>{totals.possibleIncome.toLocaleString('uk-UA')} грн.</strong></article>
      </div>

      <div className="inventory-actions panel">
        <button className="success-button" type="button" onClick={() => onNavigate('createProduct')}>
           Додати товар
        </button>
        <button className="primary-button" type="button" onClick={() => fileInputRef.current?.click()}>
           Імпортувати
        </button>
        <input ref={fileInputRef} className="hidden-file-input" type="file" accept=".csv,.xlsx" onChange={handleImport} />
        <button
          className="icon-button flat-icon-button"
          type="button"
          aria-label="Налаштування"
          onClick={() => setIsSettingsOpen(true)}
        >
          <SettingsMiniIcon />
        </button>
        {importFileName && <span className="import-file-name">Імпортовано: {importFileName}</span>}
      </div>

      <section className="products-table-card panel">
        <div className="products-layout-header">
          <span><input type="checkbox" /></span>
          <span>Товар/Послуга/Дата</span>
          <span>Доступно</span>
          <span>Продажі</span>
          <span>Собів.</span>
          <span>Ціна продажу</span>
          <span>Націнка</span>
        </div>

        {paginatedProducts.map((product) => {
          const markup = product.cost > 0 ? Math.round(((product.price - product.cost) / product.cost) * 100) : 0

          return (
            <div className="product-layout-row" key={product.id}>
              <label className="product-check"><input type="checkbox" /></label>
              <div className="product-main-cell">
                <div className="product-preview" />
                <div>
                  <strong>{product.name || "Штани Arc'teryx Gamma (легші) (1 варіант)"}</strong>
                  <span>ID: {product.sku}</span>
                  <span>Бренд: {product.brand}</span>
                  <span>Категорія: {product.category}</span>
                  <button type="button" onClick={() => setEditingProduct(product)}>Редагувати</button>
                </div>
              </div>
              <div className="large-product-cell available-cell">
                <strong>{product.stock}</strong>
                <span>({product.stock > 0 ? 1 : 0}) {product.unit}</span>
              </div>
              <div className="large-product-cell">
                <strong>0</strong>
                <span>продажів з 15.02</span>
              </div>
              <div>{product.cost.toLocaleString('uk-UA')} грн.</div>
              <div>{product.price.toLocaleString('uk-UA')} грн.</div>
              <div>{markup}%</div>
            </div>
          )
        })}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredProducts.length} onPageChange={setPage} />
      </section>

      {editingProduct && (
        <div className="modal-backdrop" role="presentation" onClick={() => setEditingProduct(null)}>
          <section className="small-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Product</p>
                <h2>Редагування товару</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setEditingProduct(null)}>×</button>
            </div>
            <div className="settings-grid">
              <label className="field">
                <span>Назва</span>
                <input readOnly value={editingProduct.name} />
              </label>
              <label className="field">
                <span>Артикул</span>
                <input readOnly value={editingProduct.sku} />
              </label>
              <label className="field">
                <span>Бренд</span>
                <input readOnly value={editingProduct.brand} />
              </label>
              <label className="field">
                <span>Ціна продажу</span>
                <input readOnly value={`${editingProduct.price} грн.`} />
              </label>
            </div>
            <div className="settings-actions">
              <button className="primary-button" type="button" onClick={() => setEditingProduct(null)}>Готово</button>
            </div>
          </section>
        </div>
      )}

      {isWarehouseModalOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsWarehouseModalOpen(false)}>
          <form className="small-modal" onSubmit={saveWarehouse} onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Warehouse</p>
                <h2>Додати склад</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setIsWarehouseModalOpen(false)}>×</button>
            </div>
            <div className="settings-grid">
              <label className="field">
                <span>Назва складу</span>
                <input
                  autoFocus
                  required
                  value={newWarehouseName}
                  placeholder="Наприклад: Склад Львів"
                  onChange={(event) => setNewWarehouseName(event.target.value)}
                />
              </label>
            </div>
            <div className="settings-actions">
              <button className="secondary-button" type="button" onClick={() => setIsWarehouseModalOpen(false)}>Скасувати</button>
              <button className="primary-button" type="submit">Додати</button>
            </div>
          </form>
        </div>
      )}

      {isSettingsOpen && (
        <div className="modal-backdrop" role="presentation" onClick={() => setIsSettingsOpen(false)}>
          <section className="small-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Inventory</p>
                <h2>Налаштування товарів</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setIsSettingsOpen(false)}>×</button>
            </div>
            <div className="settings-grid">
              <label className="field">
                <span>Склад за замовчуванням</span>
                <select value={warehouse} onChange={(event) => updateWarehouse(event.target.value)}>
                  {visibleWarehouses.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              {warehouseList.map((item, index) => (
                <label className="field" key={`${item}-${index}`}>
                  <span>Назва складу #{index + 1}</span>
                  <input value={item} onChange={(event) => renameWarehouse(index, event.target.value)} />
                </label>
              ))}
            </div>
            <div className="settings-actions">
              <button className="primary-button" type="button" onClick={() => setIsSettingsOpen(false)}>Зберегти</button>
            </div>
          </section>
        </div>
      )}
    </section>
  )
}
