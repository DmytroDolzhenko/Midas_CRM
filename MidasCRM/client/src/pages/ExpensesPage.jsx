import { useMemo, useState } from 'react'
import { Pagination } from '../components/Pagination.jsx'

const categories = ['Загальні', 'Доставка', 'Пакування', 'Маркетинг', 'Оренда']
const stores = ['Gorpcore', 'Основний склад', 'Шоурум Київ']
const accounts = ['Наложка NovaPay (6782.43 грн.)', 'Monobank ФОП', 'Готівка']
const PAGE_SIZE = 10

export function ExpensesPage({ expenses, onCreate }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [amount, setAmount] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('Загальні')
  const [store, setStore] = useState('Gorpcore')
  const [account, setAccount] = useState(accounts[0])
  const [date, setDate] = useState('2026-05-19')
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState('all')
  const [storeFilter, setStoreFilter] = useState('all')
  const [dateFrom, setDateFrom] = useState('')
  const [dateTo, setDateTo] = useState('')
  const [page, setPage] = useState(1)

  const filteredExpenses = useMemo(
    () =>
      expenses.filter((expense) => {
        const matchesSearch = `${expense.description ?? ''} ${expense.category ?? ''} ${expense.store ?? ''} ${expense.account ?? ''}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesCategory = categoryFilter === 'all' || expense.category === categoryFilter
        const matchesStore = storeFilter === 'all' || expense.store === storeFilter
        const matchesDate = (!dateFrom || expense.date >= dateFrom) && (!dateTo || expense.date <= dateTo)

        return matchesSearch && matchesCategory && matchesStore && matchesDate
      }),
    [categoryFilter, dateFrom, dateTo, expenses, search, storeFilter],
  )
  const paginatedExpenses = filteredExpenses.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  function updateFilter(setter, value) {
    setter(value)
    setPage(1)
  }

  function resetForm() {
    setAmount('')
    setDescription('')
    setCategory('Загальні')
    setStore('Gorpcore')
    setAccount(accounts[0])
    setDate('2026-05-19')
  }

  function closeModal() {
    setIsModalOpen(false)
    resetForm()
  }

  function saveExpense(status) {
    onCreate({
      amount: Number(amount),
      description,
      category,
      store,
      account,
      date,
      status,
    })
    closeModal()
  }

  function handleSubmit(event) {
    event.preventDefault()
    saveExpense('Додано')
  }

  return (
    <section className="page-stack">
      <div className="crm-page-header">
        <button className="primary-button" type="button" onClick={() => setIsModalOpen(true)}>
          + Додати витрату
        </button>
      </div>

      <section className="panel">
        <div className="table-filter-grid">
          <input
            aria-label="Пошук витрат"
            placeholder="Пошук за описом, категорією або рахунком"
            value={search}
            onChange={(event) => updateFilter(setSearch, event.target.value)}
          />
          <select value={categoryFilter} onChange={(event) => updateFilter(setCategoryFilter, event.target.value)}>
            <option value="all">Усі категорії</option>
            {categories.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <select value={storeFilter} onChange={(event) => updateFilter(setStoreFilter, event.target.value)}>
            <option value="all">Усі склади</option>
            {stores.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <input type="date" value={dateFrom} onChange={(event) => updateFilter(setDateFrom, event.target.value)} />
          <input type="date" value={dateTo} onChange={(event) => updateFilter(setDateTo, event.target.value)} />
        </div>
        <div className="table-header expenses-table">
          <span>Дата</span>
          <span>Категорія</span>
          <span>Опис</span>
          <span>Склад/Магазин</span>
          <span>Сума</span>
        </div>
        {filteredExpenses.length === 0 ? (
          <div className="expense-empty">
            <h2>Витрат поки немає</h2>
            <button className="primary-button" type="button" onClick={() => setIsModalOpen(true)}>
              Додати першу витрату
            </button>
          </div>
        ) : (
          paginatedExpenses.map((expense) => (
            <div className="table-row expenses-table" key={expense.id}>
              <span>{expense.date}</span>
              <strong>{expense.category}</strong>
              <span>{expense.description}</span>
              <span>{expense.store}</span>
              <span>{Number(expense.amount).toLocaleString('uk-UA')} грн. {expense.status === 'Заплановано' ? '(план)' : ''}</span>
            </div>
          ))
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredExpenses.length} onPageChange={setPage} />
      </section>

      {isModalOpen && (
        <div className="modal-backdrop" role="presentation" onClick={closeModal}>
          <form className="expense-modal" onSubmit={handleSubmit} onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Expense</p>
                <h2>Додати витрату</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={closeModal}>
                ×
              </button>
            </div>

            <div className="expense-form-grid">
              <label className="field">
                <span>Сума витрати</span>
                <input
                  required
                  min="0"
                  type="number"
                  placeholder="Зазначте суму витрати"
                  value={amount}
                  onChange={(event) => setAmount(event.target.value)}
                />
              </label>
              <label className="field">
                <span>Категорія витрат</span>
                <select value={category} onChange={(event) => setCategory(event.target.value)}>
                  {categories.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Склад/Магазин</span>
                <select value={store} onChange={(event) => setStore(event.target.value)}>
                  {stores.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Списати з рахунку</span>
                <select value={account} onChange={(event) => setAccount(event.target.value)}>
                  {accounts.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Дата</span>
                <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
              </label>
              <label className="field span-2">
                <span>Опис витрати</span>
                <textarea
                  rows="4"
                  placeholder="Коментар по цій витраті"
                  value={description}
                  onChange={(event) => setDescription(event.target.value)}
                />
              </label>
            </div>

            <div className="expense-modal-actions">
              <button className="link-button" type="button" onClick={() => saveExpense('Заплановано')}>
                Запланувати витрату
              </button>
              <div>
                <button className="secondary-button" type="button" onClick={closeModal}>
                  Закрити
                </button>
                <button className="primary-button" type="submit">
                  Додати витрату
                </button>
              </div>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}
