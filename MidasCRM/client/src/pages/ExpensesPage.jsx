import { useState } from 'react'

const categories = ['Загальні', 'Доставка', 'Пакування', 'Маркетинг', 'Оренда']
const stores = ['Gorpcore', 'Основний склад', 'Шоурум Київ']
const accounts = ['Наложка NovaPay (6782.43 грн.)', 'Monobank ФОП', 'Готівка']

export function ExpensesPage({ expenses, onCreate }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [amount, setAmount] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('Загальні')
  const [store, setStore] = useState('Gorpcore')
  const [account, setAccount] = useState(accounts[0])
  const [date, setDate] = useState('2026-05-19')

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
        <div className="table-header expenses-table">
          <span>Дата</span>
          <span>Категорія</span>
          <span>Опис</span>
          <span>Склад/Магазин</span>
          <span>Сума</span>
        </div>
        {expenses.length === 0 ? (
          <div className="expense-empty">
            <h2>Витрат поки немає</h2>
            <button className="primary-button" type="button" onClick={() => setIsModalOpen(true)}>
              Додати першу витрату
            </button>
          </div>
        ) : (
          expenses.map((expense) => (
            <div className="table-row expenses-table" key={expense.id}>
              <span>{expense.date}</span>
              <strong>{expense.category}</strong>
              <span>{expense.description}</span>
              <span>{expense.store}</span>
              <span>{Number(expense.amount).toLocaleString('uk-UA')} грн. {expense.status === 'Заплановано' ? '(план)' : ''}</span>
            </div>
          ))
        )}
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
