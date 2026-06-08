import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Finance.module.css'

const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')

const OPERATION_TYPES = {
  accrual: 1,
  writeOff: 2,
}

const categories = [
  { value: 1, label: 'Закупівля' },
  { value: 2, label: 'Продаж' },
  { value: 3, label: 'Реклама' },
  { value: 4, label: 'Інше' },
  { value: 5, label: 'Послуги' },
  { value: 6, label: 'Виведення коштів' },
]

const categoryLabels = new Map(categories.map((category) => [category.value, category.label]))
const PAGE_SIZE = 10

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function formatMoney(value) {
  return `${Number(value ?? 0).toLocaleString('uk-UA')} грн`
}

function formatDate(value) {
  if (!value) {
    return '-'
  }

  return new Intl.DateTimeFormat('uk-UA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

export function FinancesPage({ balance, finances = [], onCreate }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [operationType, setOperationType] = useState(OPERATION_TYPES.writeOff)
  const [amount, setAmount] = useState('')
  const [comment, setComment] = useState('')
  const [category, setCategory] = useState(4)
  const [search, setSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState('all')
  const [categoryFilter, setCategoryFilter] = useState('all')
  const [page, setPage] = useState(1)
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const activeOperations = useMemo(
    () => finances.filter((operation) => !getValue(operation, 'isDeleted', 'IsDeleted')),
    [finances],
  )

  const accrualTotal = activeOperations
    .filter((operation) => Number(getValue(operation, 'operationType', 'OperationType')) === OPERATION_TYPES.accrual)
    .reduce((sum, operation) => sum + Number(getValue(operation, 'amount', 'Amount') ?? 0), 0)

  const writeOffTotal = activeOperations
    .filter((operation) => Number(getValue(operation, 'operationType', 'OperationType')) === OPERATION_TYPES.writeOff)
    .reduce((sum, operation) => sum + Number(getValue(operation, 'amount', 'Amount') ?? 0), 0)

  const filteredFinances = useMemo(
    () =>
      activeOperations.filter((operation) => {
        const currentType = Number(getValue(operation, 'operationType', 'OperationType'))
        const currentCategory = Number(getValue(operation, 'category', 'Category'))
        const currentComment = getValue(operation, 'comment', 'Comment') ?? ''
        const matchesSearch = `${currentComment} ${categoryLabels.get(currentCategory) ?? ''}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesType = typeFilter === 'all' || currentType === Number(typeFilter)
        const matchesCategory = categoryFilter === 'all' || currentCategory === Number(categoryFilter)

        return matchesSearch && matchesType && matchesCategory
      }),
    [activeOperations, categoryFilter, search, typeFilter],
  )
  const paginatedFinances = filteredFinances.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  function updateFilter(setter, value) {
    setter(value)
    setPage(1)
  }

  function openModal(type = OPERATION_TYPES.writeOff) {
    setOperationType(type)
    setCategory(type === OPERATION_TYPES.accrual ? 4 : 1)
    setIsModalOpen(true)
  }

  function resetForm() {
    setAmount('')
    setComment('')
    setCategory(4)
    setOperationType(OPERATION_TYPES.writeOff)
    setError('')
  }

  function closeModal() {
    setIsModalOpen(false)
    resetForm()
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await onCreate({
        operationType,
        category: Number(category),
        amount: Number(amount),
        comment,
        orderId: null,
      })
      closeModal()
    } catch (submitError) {
      setError(submitError.message || 'Не вдалося зберегти операцію')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className={cx('page-stack')}>
      <div className={cx('crm-page-header')}>
        <div className={cx('finance-actions')}>
          <button className={cx('primary-button')} type="button" onClick={() => openModal()}>
            Створити фінансову операцію
          </button>
        </div>
      </div>

      <div className={cx('account-grid')}>
        <article className={cx('account-card')}>
          <span>Поточний баланс</span>
          <strong>{formatMoney(getValue(balance, 'balance', 'Balance'))}</strong>
          <small>Дані поточного балансу компанії</small>
        </article>
        <article className={cx('account-card')}>
          <span>Поповнення</span>
          <strong>{formatMoney(accrualTotal)}</strong>
          <small>Ручні та автоматичні нарахування</small>
        </article>
        <article className={cx('account-card')}>
          <span>Витрати</span>
          <strong>{formatMoney(writeOffTotal)}</strong>
          <small>Списання з рахунку</small>
        </article>
      </div>

      <section className={cx('panel')}>
        <div className={cx('table-filter-grid')}>
          <input
            aria-label="Пошук фінансових операцій"
            placeholder="Пошук за коментарем або категорією"
            value={search}
            onChange={(event) => updateFilter(setSearch, event.target.value)}
          />
          <select value={typeFilter} onChange={(event) => updateFilter(setTypeFilter, event.target.value)}>
            <option value="all">Усі типи</option>
            <option value={OPERATION_TYPES.accrual}>Поповнення</option>
            <option value={OPERATION_TYPES.writeOff}>Витрати</option>
          </select>
          <select value={categoryFilter} onChange={(event) => updateFilter(setCategoryFilter, event.target.value)}>
            <option value="all">Усі категорії</option>
            {categories.map((item) => (
              <option key={item.value} value={item.value}>{item.label}</option>
            ))}
          </select>
        </div>

        <div className={cx('table-header', 'expenses-table')}>
          <span>Дата</span>
          <span>Тип</span>
          <span>Коментар</span>
          <span>Категорія</span>
          <span>Сума</span>
        </div>
        {filteredFinances.length === 0 ? (
          <div className={cx('expense-empty')}>
            <h2>Фінансових операцій поки немає</h2>
            <button className={cx('primary-button')} type="button" onClick={() => openModal()}>
              Створити фінансову операцію
            </button>
          </div>
        ) : (
          paginatedFinances.map((operation) => {
            const currentType = Number(getValue(operation, 'operationType', 'OperationType'))
            const currentCategory = Number(getValue(operation, 'category', 'Category'))
            const amountValue = Number(getValue(operation, 'amount', 'Amount') ?? 0)

            return (
              <div className={cx('table-row', 'expenses-table')} key={String(getValue(operation, 'id', 'Id'))}>
                <span>{formatDate(getValue(operation, 'createdAt', 'CreatedAt'))}</span>
                <strong>{currentType === OPERATION_TYPES.accrual ? 'Поповнення' : 'Витрата'}</strong>
                <span>{getValue(operation, 'comment', 'Comment') || '-'}</span>
                <span>{categoryLabels.get(currentCategory) ?? currentCategory}</span>
                <span className={currentType === OPERATION_TYPES.accrual ? cx('amount-positive') : cx('amount-negative')}>
                  {currentType === OPERATION_TYPES.accrual ? '+' : '-'}{formatMoney(amountValue)}
                </span>
              </div>
            )
          })
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredFinances.length} onPageChange={setPage} />
      </section>

      {isModalOpen && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={closeModal}>
          <form className={cx('expense-modal')} onSubmit={handleSubmit} onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div>
                <p className="eyebrow">{operationType === OPERATION_TYPES.accrual ? 'Top up' : 'Expense'}</p>
                <h2>{operationType === OPERATION_TYPES.accrual ? 'Поповнити рахунок' : 'Додати витрату'}</h2>
              </div>
              <button className={cx('modal-close-button')} type="button" onClick={closeModal}>
                ×
              </button>
            </div>

            <div className={cx('expense-form-grid')}>
              <label className={cx('field')}>
                <span>Тип операції</span>
                <select value={operationType} onChange={(event) => setOperationType(Number(event.target.value))}>
                  <option value={OPERATION_TYPES.accrual}>Поповнення</option>
                  <option value={OPERATION_TYPES.writeOff}>Витрата</option>
                </select>
              </label>
              <label className={cx('field')}>
                <span>Сума</span>
                <input
                  required
                  min="0"
                  step="0.01"
                  type="number"
                  placeholder="Вкажіть суму"
                  value={amount}
                  onChange={(event) => setAmount(event.target.value)}
                />
              </label>
              <label className={cx('field')}>
                <span>Категорія</span>
                <select value={category} onChange={(event) => setCategory(Number(event.target.value))}>
                  {categories.map((item) => (
                    <option key={item.value} value={item.value}>{item.label}</option>
                  ))}
                </select>
              </label>
              <label className={cx('field', 'span-2')}>
                <span>Коментар</span>
                <textarea
                  rows="4"
                  placeholder="Коментар до фінансової операції"
                  value={comment}
                  onChange={(event) => setComment(event.target.value)}
                />
              </label>
            </div>

            {error && <p className="form-error">{error}</p>}

            <div className={cx('expense-modal-actions')}>
              <span />
              <div>
                <button className={cx('secondary-button')} type="button" onClick={closeModal}>
                  Закрити
                </button>
                <button className={cx('primary-button')} type="submit" disabled={isSubmitting || !amount}>
                  Зберегти
                </button>
              </div>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}
