
import sharedStyles from '../../../styles/Shared.module.css'
import pageStyles from '../../../styles/pages/Operations.module.css'

const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')

export function OperationsTable({ operations }) {
  return (
    <div className={cx('operations-list')}>
      {operations.map((operation) => (
        <article className={cx('operation-item')} key={operation.id}>
          <time>{operation.createdAt}</time>
          <strong>{operation.type}</strong>
          <span>{operation.description}</span>
          <em>{operation.actor} / {operation.amount}</em>
        </article>
      ))}
    </div>
  )
}
