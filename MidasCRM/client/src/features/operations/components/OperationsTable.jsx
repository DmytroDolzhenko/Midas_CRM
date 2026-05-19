export function OperationsTable({ operations }) {
  return (
    <div className="operations-list">
      {operations.map((operation) => (
        <article className="operation-item" key={operation.id}>
          <time>{operation.createdAt}</time>
          <strong>{operation.type}</strong>
          <span>{operation.description}</span>
          <em>{operation.actor} / {operation.amount}</em>
        </article>
      ))}
    </div>
  )
}
