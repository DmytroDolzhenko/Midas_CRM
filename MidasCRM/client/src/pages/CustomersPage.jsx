import { useState } from 'react'
import { CustomersTable } from '../features/customers/components/CustomersTable.jsx'

export function CustomersPage({ customers = [] }) {
  const [search, setSearch] = useState('')

  const filteredCustomers = customers.filter((customer) => {
    const searchString = `${customer.name ?? ''} ${customer.phone ?? ''} ${customer.email ?? ''}`.toLowerCase()
    return searchString.includes(search.toLowerCase())
  })

  return (
    <section className="page-stack">
      <section className="panel">
        <div className="toolbar">
          <input
            aria-label="Пошук клієнтів"
            placeholder="Пошук за ім'ям, телефоном або поштою"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
          />
        </div>
        <CustomersTable customers={filteredCustomers} />
      </section>
    </section>
  )
}
