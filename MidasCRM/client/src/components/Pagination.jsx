export function Pagination({ page, pageSize, total, onPageChange }) {
  const pageCount = Math.max(1, Math.ceil(total / pageSize))
  const firstItem = total === 0 ? 0 : (page - 1) * pageSize + 1
  const lastItem = Math.min(page * pageSize, total)

  if (total <= pageSize) {
    return null
  }

  return (
    <div className="pagination">
      <span>
        {firstItem}-{lastItem} з {total}
      </span>
      <div>
        <button
          className="secondary-button"
          type="button"
          disabled={page === 1}
          onClick={() => onPageChange(page - 1)}
        >
          Назад
        </button>
        <strong>{page} / {pageCount}</strong>
        <button
          className="secondary-button"
          type="button"
          disabled={page === pageCount}
          onClick={() => onPageChange(page + 1)}
        >
          Далі
        </button>
      </div>
    </div>
  )
}
