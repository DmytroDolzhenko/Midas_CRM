const labels = {
  draft: 'Чернетка',
  processing: 'В роботі',
  completed: 'Завершено',
  cancelled: 'Скасовано',
  0: 'Очікує',
  1: 'В обробці',
  2: 'Відправлено',
  3: 'Доставлено',
  4: 'Повернення',
  5: 'Отримано',
  6: 'Скасовано',
}

const classes = {
  draft: 'draft',
  processing: 'processing',
  completed: 'completed',
  cancelled: 'cancelled',
  0: 'draft',
  1: 'processing',
  2: 'processing',
  3: 'completed',
  4: 'cancelled',
  5: 'completed',
  6: 'cancelled',
}

export function StatusBadge({ status }) {
  return <span className={`status-badge status-${classes[status] ?? 'draft'}`}>{labels[status] ?? status}</span>
}
