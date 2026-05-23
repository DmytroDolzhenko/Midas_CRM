import { StatusBadge } from '../../../components/StatusBadge.jsx'
import sharedStyles from '../../../styles/Shared.module.css'


const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')



export function OrdersTable({ orders }) {
  return (
    <>
      <div className={cx('table-header', 'sales-table')}>
        <span>Номер</span>
        <span>Клієнт</span>
        <span>Канал</span>
        <span>Сума</span>
        <span>Статус</span>
      </div>
      {orders.map((order) => (
        <div className={cx('table-row', 'sales-table')} key={order.id}>
          <strong>{order.code}</strong>
          <span>{order.customer}</span>
          <span>{order.channel}</span>
          <span>{order.total.toLocaleString('uk-UA')} грн</span>
          <StatusBadge status={order.status} />
        </div>
      ))}
    </>
  )
}
