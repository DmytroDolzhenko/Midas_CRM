import { useMemo, useRef, useState } from 'react'
import { Pagination } from '../components/Pagination.jsx'

const MAIN_WAREHOUSE = 'Основний склад'
const PAGE_SIZE = 8
const stockFilters = [
  { id: 'all', label: 'всі' },
  { id: 'available', label: 'в наявності' },
  { id: 'empty', label: 'немає в наявності' },
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

export function ProductsPage({ products, onNavigate }) {
  const [search, setSearch] = useState('')
  const [stockFilter, setStockFilter] = useState('all')
  const [isFilterOpen, setIsFilterOpen] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null)
  const [importFileName, setImportFileName] = useState('')
  const [minCost, setMinCost] = useState('')
  const [maxCost, setMaxCost] = useState('')
  const [minPrice, setMinPrice] = useState('')
  const [maxPrice, setMaxPrice] = useState('')
  const [page, setPage] = useState(1)
  const fileInputRef = useRef(null)

  const mainWarehouseProducts = useMemo(
    () => products.filter((product) => product.warehouse === MAIN_WAREHOUSE),
    [products],
  )

  const filteredProducts = useMemo(
    () =>
      mainWarehouseProducts.filter((product) => {
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
    [mainWarehouseProducts, maxCost, maxPrice, minCost, minPrice, search, stockFilter],
  )

  const paginatedProducts = filteredProducts.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const totals = useMemo(() => {
    const available = mainWarehouseProducts.filter((product) => product.stock > 0)
    const stockUnits = mainWarehouseProducts.reduce((sum, product) => sum + Number(product.stock), 0)
    const storedValue = mainWarehouseProducts.reduce((sum, product) => sum + product.stock * product.cost, 0)
    const possibleIncome = mainWarehouseProducts.reduce((sum, product) => sum + product.stock * (product.price - product.cost), 0)

    return {
      total: mainWarehouseProducts.length,
      available: available.length,
      stockUnits,
      storedValue,
      possibleIncome,
    }
  }, [mainWarehouseProducts])

  function updateSearch(value) {
    setSearch(value)
    setPage(1)
  }

  function updateStockFilter(value) {
    setStockFilter(value)
    setPage(1)
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
          <button type="button" className="warehouse-tab active">
            {MAIN_WAREHOUSE}
          </button>
        </div>

        <div className="catalog-filters">
          <input
            aria-label="Пошукова фраза"
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
        <article><span>Зберігається товарів на</span><strong>{totals.storedValue.toLocaleString('uk-UA')} грн</strong></article>
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
                  <button type="button" onClick={() => setEditingProduct(product)}>Редагувати</button>
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
                <h2>Редагування товару</h2>
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
                <input readOnly value={MAIN_WAREHOUSE} />
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
    </section>
  )
}
