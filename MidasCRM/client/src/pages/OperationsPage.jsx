import { OperationsTable } from '../features/operations/components/OperationsTable.jsx'

export function OperationsPage({ operations }) {
  return (
    <section className="page-stack">
      <section className="panel">
        <OperationsTable operations={operations} />
      </section>
    </section>
  )
}
