import { Button } from '../components/Button.jsx'
import { ProductsTable } from '../features/sales/components/ProductsTable.jsx'
import { useSalesSearch } from '../features/sales/hooks/useSalesSearch.js'

export function ProductsPage({ products, onNavigate }) {
  const { search, setSearch, filteredItems } = useSalesSearch(
    products,
    (product) => `${product.sku} ${product.name} ${product.category}`,
  )

  return (
    <section className="page-stack">
      <div className="page-header">
        <Button onClick={() => onNavigate('createProduct')}>Додати товар</Button>
      </div>

      <section className="panel">
        <div className="toolbar">
          <input
            aria-label="Пошук товарів"
            placeholder="Пошук за SKU, назвою або категорією"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>
        <ProductsTable products={filteredItems} />
      </section>
    </section>
  )
}
