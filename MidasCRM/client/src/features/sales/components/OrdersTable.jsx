import { StatusBadge } from '../../../components/StatusBadge.jsx'

export function OrdersTable({ orders }) {
  return (
    <>
      <div className="table-header orders-table">
        <span>Код</span>
        <span>Клієнт</span>
        <span>Сума</span>
        <span>Статус</span>
      </div>
      {orders.map((order) => (
        <div className="table-row orders-table" key={order.id}>
          <strong>{order.code}</strong>
          <span>{order.customer}</span>
          <span>{order.total.toLocaleString('uk-UA')} грн</span>
          <StatusBadge status={order.status} />
        </div>
      ))}
    </>
  )
}
