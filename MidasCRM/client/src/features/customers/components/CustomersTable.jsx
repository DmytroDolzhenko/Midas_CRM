export function CustomersTable({ customers }) {
  return (
    <section className="panel">
      <div className="table-header customers-table">
        <span>Ім’я</span>
        <span>Email</span>
        <span>Телефон</span>
      </div>
      {customers.map((customer) => (
        <div className="table-row customers-table" key={customer.id}>
          <strong>{customer.name}</strong>
          <span>{customer.email}</span>
          <span>{customer.phone}</span>
        </div>
      ))}
    </section>
  )
}
