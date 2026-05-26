import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import { CustomersTable } from '../../features/customers/components/CustomersTable.jsx'
import sharedStyles from '../../styles/Shared.module.css'

const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')

const PAGE_SIZE = 10

export function CustomersPage({ customers = [], onNavigate, onDelete }) {
  const [search, setSearch] = useState('')
  const [hasEmail, setHasEmail] = useState('all')
  const [hasPhone, setHasPhone] = useState('all')
  const [page, setPage] = useState(1)

  const filteredCustomers = useMemo(
    () =>
      customers.filter((customer) => {
        const searchString = `${customer.name ?? ''} ${customer.firstName ?? ''} ${customer.surname ?? ''} ${customer.phone ?? ''} ${customer.email ?? ''}`.toLowerCase()
        const matchesSearch = searchString.includes(search.toLowerCase())
        const matchesEmail = hasEmail === 'all' || (hasEmail === 'yes' ? Boolean(customer.email) : !customer.email)
        const matchesPhone = hasPhone === 'all' || (hasPhone === 'yes' ? Boolean(customer.phone) : !customer.phone)

        return matchesSearch && matchesEmail && matchesPhone
      }),
    [customers, hasEmail, hasPhone, search],
  )
  const paginatedCustomers = filteredCustomers.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  function updateFilter(setter, value) {
    setter(value)
    setPage(1)
  }

  return (
    <section className={cx('page-stack')}>
      <div className={cx('page-header')}>
        <button className={cx('primary-button')} type="button" onClick={() => onNavigate?.('createCustomer')}>
          Створити клієнта
        </button>
      </div>

      <section className={cx('panel')}>
        <div className={cx('table-filter-grid')}>
          <input
            aria-label="Пошук клієнтів"
            placeholder="Пошук за імʼям, телефоном або поштою"
            value={search}
            onChange={(event) => updateFilter(setSearch, event.target.value)}
          />
          <select value={hasEmail} onChange={(event) => updateFilter(setHasEmail, event.target.value)}>
            <option value="all">Усі email</option>
            <option value="yes">Є email</option>
            <option value="no">Без email</option>
          </select>
          <select value={hasPhone} onChange={(event) => updateFilter(setHasPhone, event.target.value)}>
            <option value="all">Усі телефони</option>
            <option value="yes">Є телефон</option>
            <option value="no">Без телефону</option>
          </select>
        </div>
        <CustomersTable customers={paginatedCustomers} onDelete={onDelete} />
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredCustomers.length} onPageChange={setPage} />
      </section>
    </section>
  )
}
