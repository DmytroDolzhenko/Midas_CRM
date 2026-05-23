import { useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import { CustomersTable } from '../../features/customers/components/CustomersTable.jsx'
import sharedStyles from '../../styles/Shared.module.css'


const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')



const PAGE_SIZE = 10

export function CustomersPage({ customers = [] }) {
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)

  const filteredCustomers = customers.filter((customer) => {
    const searchString = `${customer.name ?? ''} ${customer.phone ?? ''} ${customer.email ?? ''}`.toLowerCase()
    return searchString.includes(search.toLowerCase())
  })
  const paginatedCustomers = filteredCustomers.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  return (
    <section className={cx('page-stack')}>
      <section className={cx('panel')}>
        <div className={cx('toolbar')}>
          <input
            aria-label="Пошук клієнтів"
            placeholder="Пошук за ім'ям, телефоном або поштою"
            value={search}
            onChange={(event) => { setSearch(event.target.value); setPage(1) }}
          />
        </div>
        <CustomersTable customers={paginatedCustomers} />
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredCustomers.length} onPageChange={setPage} />
      </section>
    </section>
  )
}

