import { MetricCard } from '../components/MetricCard.jsx'
import { OrdersTable } from '../features/sales/components/OrdersTable.jsx'

export function DashboardPage({ stats, recentOrders }) {
  const averageOrder = stats.orders > 0 ? Math.round(stats.revenue / stats.orders) : 0
  const conversionRate = stats.customers > 0 ? Math.round((stats.orders / stats.customers) * 100) : 0

  return (
    <section className="page-stack">

      <div className="metric-grid">
        <MetricCard
          label="Оборот"
          value={`${stats.revenue.toLocaleString('uk-UA')} грн`}
          hint="Сума всіх замовлень"
        />
        <MetricCard label="Замовлення" value={stats.orders} hint="У роботі та завершені" />
        <MetricCard label="Клієнти" value={stats.customers} hint="Контакти в базі" />
        <MetricCard label="Нові повідомлення" value={stats.unreadMessages} hint="Instagram, OLX та інші" />
      </div>

      <section className="panel">
        <h2>Останні замовлення</h2>
        <OrdersTable orders={recentOrders} />
      </section>
    </section>
  )
}
