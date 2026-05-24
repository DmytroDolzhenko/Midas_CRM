import sharedStyles from '../../../styles/Shared.module.css'

const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')

export function CustomersTable({ customers, onDelete }) {
  return (
    <section>
      <div className={cx('table-header', 'customers-table')}>
        <span>Імʼя</span>
        <span>Email</span>
        <span>Телефон</span>
        <span>Дії</span>
      </div>
      {!customers.length && (
        <div className={cx('table-row')}>
          <span>Клієнтів за цими фільтрами не знайдено</span>
        </div>
      )}
      {customers.map((customer) => (
        <div className={cx('table-row', 'customers-table')} key={customer.id}>
          <strong>{customer.name}</strong>
          <span>{customer.email || '-'}</span>
          <span>{customer.phone || '-'}</span>
          <span>
            <button className={cx('secondary-button')} type="button" onClick={() => onDelete?.(customer.id)}>
              Видалити
            </button>
          </span>
        </div>
      ))}
    </section>
  )
}
