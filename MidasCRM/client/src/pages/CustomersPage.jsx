import { CustomersTable } from '../features/customers/components/CustomersTable.jsx'

export function CustomersPage({ customers }) {
  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Customers</p>
          <h1>Клієнти</h1>
        </div>
      </div>

      <CustomersTable customers={customers} />
    </section>
  )
}
