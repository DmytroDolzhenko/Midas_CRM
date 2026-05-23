import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Finance.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



const categories = ['Загальні', 'Доставка', 'Пакування', 'Маркетинг', 'Оренда']
const stores = ['Основний склад']
const accounts = ['NovaPay', 'Monobank', 'Готівка']
const PAGE_SIZE = 10

export function FinancesPage({ finances, onCreate }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [amount, setAmount] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('Загальні')
  const [store, setStore] = useState('Основний склад')
  const [account, setAccount] = useState(accounts[0])
  const [date, setDate] = useState('2026-05-19')
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState('all')
  const [storeFilter, setStoreFilter] = useState('all')
  const [dateFrom, setDateFrom] = useState('')
  const [dateTo, setDateTo] = useState('')
  const [page, setPage] = useState(1)

  const filteredFinances = useMemo(
    () =>
      finances.filter((finance) => {
        const matchesSearch = `${finance.description ?? ''} ${finance.category ?? ''} ${finance.store ?? ''} ${finance.account ?? ''}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesCategory = categoryFilter === 'all' || finance.category === categoryFilter
        const matchesStore = storeFilter === 'all' || finance.store === storeFilter
        const matchesDate = (!dateFrom || finance.date >= dateFrom) && (!dateTo || finance.date <= dateTo)

        return matchesSearch && matchesCategory && matchesStore && matchesDate
      }),
    [categoryFilter, dateFrom, dateTo, finances, search, storeFilter],
  )
  const paginatedFinances = filteredFinances.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  function updateFilter(setter, value) {
    setter(value)
    setPage(1)
  }

  function resetForm() {
    setAmount('')
    setDescription('')
    setCategory('Загальні')
    setStore('Основний склад')
    setAccount(accounts[0])
    setDate('2026-05-19')
  }

  function closeModal() {
    setIsModalOpen(false)
    resetForm()
  }

  function saveFinances(status) {
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
    saveFinances('Додано')
  }

  return (
    <section className={cx('page-stack')}>
      <div className={cx('crm-page-header')}>
        <button className={cx('primary-button')} type="button" onClick={() => setIsModalOpen(true)}>
          Додати фінансову операцію
        </button>
      </div>

      <section className={cx('panel')}>
        <div className={cx('table-filter-grid')}>
          <input
            aria-label="Пошук фінансових операцій"
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
        <div className={cx('table-header', 'expenses-table')}>
          <span>Дата</span>
          <span>Категорія</span>
          <span>Опис</span>
          <span>Склад/Магазин</span>
          <span>Сума</span>
        </div>
        {filteredFinances.length === 0 ? (
          <div className={cx('expense-empty')}>
            <h2>Фінансові операції поки немає</h2>
            <button className={cx('primary-button')} type="button" onClick={() => setIsModalOpen(true)}>
              Додати першу фінансову операцію
            </button>
          </div>
        ) : (
          paginatedFinances.map((finance) => (
            <div className={cx('table-row', 'expenses-table')} key={finance.id}>
              <span>{finance.date}</span>
              <strong>{finance.category}</strong>
              <span>{finance.description}</span>
              <span>{finance.store}</span>
              <span>{Number(finance.amount).toLocaleString('uk-UA')} грн. {finance.status === 'Заплановано' ? '(план)' : ''}</span>
            </div>
          ))
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredFinances.length} onPageChange={setPage} />
      </section>

      {isModalOpen && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={closeModal}>
          <form className={cx('expense-modal')} onSubmit={handleSubmit} onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div>
                <p className={cx('eyebrow')}>Expense</p>
                <h2>Додати витрату</h2>
              </div>
              <button className={cx('modal-close-button')} type="button" onClick={closeModal}>
                ×
              </button>
            </div>

            <div className={cx('expense-form-grid')}>
              <label className={cx('field')}>
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
              <label className={cx('field')}>
                <span>Категорія витрат</span>
                <select value={category} onChange={(event) => setCategory(event.target.value)}>
                  {categories.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className={cx('field')}>
                <span>Склад/Магазин</span>
                <select value={store} onChange={(event) => setStore(event.target.value)}>
                  {stores.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className={cx('field')}>
                <span>Списати з рахунку</span>
                <select value={account} onChange={(event) => setAccount(event.target.value)}>
                  {accounts.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className={cx('field')}>
                <span>Дата</span>
                <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
              </label>
              <label className={cx('field', 'span-2')}>
                <span>Опис витрати</span>
                <textarea
                  rows="4"
                  placeholder="Коментар по цій витраті"
                  value={description}
                  onChange={(event) => setDescription(event.target.value)}
                />
              </label>
            </div>

            <div className={cx('expense-modal-actions')}>
              <button className={cx('link-button')} type="button" onClick={() => saveFinances('Заплановано')}>
                Запланувати витрату
              </button>
              <div>
                <button className={cx('secondary-button')} type="button" onClick={closeModal}>
                  Закрити
                </button>
                <button className={cx('primary-button')} type="submit">
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

