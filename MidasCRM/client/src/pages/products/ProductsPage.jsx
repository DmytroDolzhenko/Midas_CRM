import { useMemo, useRef, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'

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
        const matchesSearch = `${product.name} ${product.sku} ${product.category}`
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
    <section className="catalog-page">
      <div className="catalog-toolbar panel">
        <div className="warehouse-tabs">
          {warehouses.map((warehouse) => {
            const warehouseId = String(getValue(warehouse, 'id', 'Id'))
            return (
              <button
                key={warehouseId}
                type="button"
                className={warehouseId === String(selectedWarehouseId) ? 'warehouse-tab active' : 'warehouse-tab'}
                onClick={() => updateWarehouse(warehouseId)}
              >
                {getValue(warehouse, 'name', 'Name')}
              </button>
            )
          })}
          <button className="warehouse-add-button" type="button" onClick={openCreateWarehouse}>+</button>
          <button className="dots-button" type="button" onClick={openEditWarehouse}>...</button>
        </div>

        <div className="catalog-filters">
          <input
            aria-label="Пошук товарів"
            placeholder="Пошук товарів"
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
        <article><span>Усього товару</span><strong>{totals.total}</strong></article>
        <article><span>В наявності</span><strong>{totals.available} товарів</strong></article>
        <article><span>Доступно</span><strong>{totals.stockUnits.toFixed(2)} одиниць</strong></article>
        <article><span>Товарів на складі</span><strong>{totals.storedValue.toLocaleString('uk-UA')} грн</strong></article>
        <article><span>Можливий дохід</span><strong>{totals.possibleIncome.toLocaleString('uk-UA')} грн</strong></article>
      </div>

      <div className="inventory-actions panel">
        <button className="success-button" type="button" onClick={() => onNavigate('createProduct')}>
          Додати товар
        </button>
        <button className="primary-button" type="button" onClick={() => fileInputRef.current?.click()}>
          Імпортувати
        </button>
        <input ref={fileInputRef} className="hidden-file-input" type="file" accept=".csv,.xlsx" onChange={handleImport} />
        {importFileName && <span className="import-file-name">Імпортовано: {importFileName}</span>}
      </div>

      <section className="products-table-card panel">
        <div className="products-layout-header">
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
            <div className="product-layout-row" key={product.id}>
              <label className="product-check"><input type="checkbox" /></label>
              <div className="product-main-cell">
                <div className="product-preview" />
                <div>
                  <strong>{product.name}</strong>
                  <span>ID: {product.sku}</span>
                  <span>Категорія: {product.category}</span>
                  <button type="button" onClick={() => setEditingProduct(product)}>� едагувати</button>
                </div>
              </div>
              <div className="large-product-cell available-cell">
                <strong>{product.stock}</strong>
                <span>{product.unit}</span>
              </div>
              <div className="large-product-cell">
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
        <div className="modal-backdrop" role="presentation" onClick={() => setEditingProduct(null)}>
          <section className="small-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Product</p>
                <h2>� едагування товару</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setEditingProduct(null)}>x</button>
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
                <span>Склад</span>
                <input readOnly value={editingProduct.warehouse} />
              </label>
              <label className="field">
                <span>Ціна продажу</span>
                <input readOnly value={`${editingProduct.price} грн`} />
              </label>
            </div>
            <div className="settings-actions">
              <button className="primary-button" type="button" onClick={() => setEditingProduct(null)}>Готово</button>
            </div>
          </section>
        </div>
      )}

      {editingWarehouse && (
        <div className="modal-backdrop" role="presentation" onClick={() => setEditingWarehouse(null)}>
          <form className="small-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()} onSubmit={submitWarehouse}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Warehouse</p>
                <h2>{editingWarehouse.mode === 'edit' ? '� едагувати склад' : 'Новий склад'}</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setEditingWarehouse(null)}>x</button>
            </div>
            <div className="settings-grid">
              <label className="field span-2">
                <span>Назва складу</span>
                <input value={warehouseName} maxLength="100" onChange={(event) => setWarehouseName(event.target.value)} />
              </label>
            </div>
            {warehouseError && <p className="settings-message">{warehouseError}</p>}
            <div className="settings-actions">
              <button className="secondary-button" type="button" onClick={() => setEditingWarehouse(null)}>Скасувати</button>
              <button className="primary-button" type="submit">Зберегти</button>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}


