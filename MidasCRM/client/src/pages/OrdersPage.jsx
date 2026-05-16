import { Button } from '../components/Button.jsx'
import { OrdersTable } from '../features/sales/components/OrdersTable.jsx'
import { useSalesSearch } from '../features/sales/hooks/useSalesSearch.js'

export function OrdersPage({ orders, onNavigate }) {
  const { search, setSearch, filteredItems } = useSalesSearch(
    orders,
    (order) => `${order.code} ${order.customer} ${order.product}`,
  )

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Orders</p>
          <h1>Замовлення</h1>
        </div>
        <Button onClick={() => onNavigate('createOrder')}>Нове замовлення</Button>
      </div>

      <section className="panel">
        <div className="toolbar">
          <input
            aria-label="Пошук замовлень"
            placeholder="Пошук за кодом, клієнтом або товаром"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>
        <OrdersTable orders={filteredItems} />
      </section>
    </section>
  )
}
