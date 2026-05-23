
import sharedStyles from '../../../styles/Shared.module.css'

const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')

export function CustomersTable({ customers }) {
  return (
    <section>
      <div className={cx('table-header', 'customers-table')}>
        <span>Ім’я</span>
        <span>Email</span>
        <span>Телефон</span>
      </div>
      {customers.map((customer) => (
        <div className={cx('table-row', 'customers-table')} key={customer.id}>
          <strong>{customer.name}</strong>
          <span>{customer.email}</span>
          <span>{customer.phone}</span>
        </div>
      ))}
    </section>
  )
}
