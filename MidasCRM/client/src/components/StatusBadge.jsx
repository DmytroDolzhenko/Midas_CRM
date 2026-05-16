const labels = {
  draft: 'Чернетка',
  processing: 'В роботі',
  completed: 'Завершено',
  cancelled: 'Скасовано',
}

export function StatusBadge({ status }) {
  return <span className={`status-badge status-${status}`}>{labels[status]}</span>
}
