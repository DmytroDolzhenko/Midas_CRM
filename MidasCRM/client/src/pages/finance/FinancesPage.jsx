import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'

const categories = ['Р—Р°РіР°Р»СЊРЅС–', 'Р”РѕСЃС‚Р°РІРєР°', 'РџР°РєСѓРІР°РЅРЅСЏ', 'РњР°СЂРєРµС‚РёРЅРі', 'РћСЂРµРЅРґР°']
const stores = ['РћСЃРЅРѕРІРЅРёР№ СЃРєР»Р°Рґ']
const accounts = ['NovaPay', 'Monobank', 'Р“РѕС‚С–РІРєР°']
const PAGE_SIZE = 10

export function FinancesPage({ finances, onCreate }) {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [amount, setAmount] = useState('')
  const [description, setDescription] = useState('')
  const [category, setCategory] = useState('Р—Р°РіР°Р»СЊРЅС–')
  const [store, setStore] = useState('РћСЃРЅРѕРІРЅРёР№ СЃРєР»Р°Рґ')
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
    setCategory('Р—Р°РіР°Р»СЊРЅС–')
    setStore('РћСЃРЅРѕРІРЅРёР№ СЃРєР»Р°Рґ')
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
    saveFinances('Р”РѕРґР°РЅРѕ')
  }

  return (
    <section className="page-stack">
      <div className="crm-page-header">
        <button className="primary-button" type="button" onClick={() => setIsModalOpen(true)}>
          Р”РѕРґР°С‚Рё С„С–РЅР°РЅСЃРѕРІСѓ РѕРїРµСЂР°С†С–СЋ
        </button>
      </div>

      <section className="panel">
        <div className="table-filter-grid">
          <input
            aria-label="РџРѕС€СѓРє С„С–РЅР°РЅСЃРѕРІРёС… РѕРїРµСЂР°С†С–Р№"
            placeholder="РџРѕС€СѓРє Р·Р° РѕРїРёСЃРѕРј, РєР°С‚РµРіРѕСЂС–С”СЋ Р°Р±Рѕ СЂР°С…СѓРЅРєРѕРј"
            value={search}
            onChange={(event) => updateFilter(setSearch, event.target.value)}
          />
          <select value={categoryFilter} onChange={(event) => updateFilter(setCategoryFilter, event.target.value)}>
            <option value="all">РЈСЃС– РєР°С‚РµРіРѕСЂС–С—</option>
            {categories.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <select value={storeFilter} onChange={(event) => updateFilter(setStoreFilter, event.target.value)}>
            <option value="all">РЈСЃС– СЃРєР»Р°РґРё</option>
            {stores.map((item) => (
              <option key={item} value={item}>{item}</option>
            ))}
          </select>
          <input type="date" value={dateFrom} onChange={(event) => updateFilter(setDateFrom, event.target.value)} />
          <input type="date" value={dateTo} onChange={(event) => updateFilter(setDateTo, event.target.value)} />
        </div>
        <div className="table-header expenses-table">
          <span>Р”Р°С‚Р°</span>
          <span>РљР°С‚РµРіРѕСЂС–СЏ</span>
          <span>РћРїРёСЃ</span>
          <span>РЎРєР»Р°Рґ/РњР°РіР°Р·РёРЅ</span>
          <span>РЎСѓРјР°</span>
        </div>
        {filteredFinances.length === 0 ? (
          <div className="expense-empty">
            <h2>Р¤С–РЅР°РЅСЃРѕРІС– РѕРїРµСЂР°С†С–С— РїРѕРєРё РЅРµРјР°С”</h2>
            <button className="primary-button" type="button" onClick={() => setIsModalOpen(true)}>
              Р”РѕРґР°С‚Рё РїРµСЂС€Сѓ С„С–РЅР°РЅСЃРѕРІСѓ РѕРїРµСЂР°С†С–СЋ
            </button>
          </div>
        ) : (
          paginatedFinances.map((finance) => (
            <div className="table-row expenses-table" key={finance.id}>
              <span>{finance.date}</span>
              <strong>{finance.category}</strong>
              <span>{finance.description}</span>
              <span>{finance.store}</span>
              <span>{Number(finance.amount).toLocaleString('uk-UA')} РіСЂРЅ. {finance.status === 'Р—Р°РїР»Р°РЅРѕРІР°РЅРѕ' ? '(РїР»Р°РЅ)' : ''}</span>
            </div>
          ))
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredFinances.length} onPageChange={setPage} />
      </section>

      {isModalOpen && (
        <div className="modal-backdrop" role="presentation" onClick={closeModal}>
          <form className="expense-modal" onSubmit={handleSubmit} onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Expense</p>
                <h2>Р”РѕРґР°С‚Рё РІРёС‚СЂР°С‚Сѓ</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={closeModal}>
                Г—
              </button>
            </div>

            <div className="expense-form-grid">
              <label className="field">
                <span>РЎСѓРјР° РІРёС‚СЂР°С‚Рё</span>
                <input
                  required
                  min="0"
                  type="number"
                  placeholder="Р—Р°Р·РЅР°С‡С‚Рµ СЃСѓРјСѓ РІРёС‚СЂР°С‚Рё"
                  value={amount}
                  onChange={(event) => setAmount(event.target.value)}
                />
              </label>
              <label className="field">
                <span>РљР°С‚РµРіРѕСЂС–СЏ РІРёС‚СЂР°С‚</span>
                <select value={category} onChange={(event) => setCategory(event.target.value)}>
                  {categories.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>РЎРєР»Р°Рґ/РњР°РіР°Р·РёРЅ</span>
                <select value={store} onChange={(event) => setStore(event.target.value)}>
                  {stores.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>РЎРїРёСЃР°С‚Рё Р· СЂР°С…СѓРЅРєСѓ</span>
                <select value={account} onChange={(event) => setAccount(event.target.value)}>
                  {accounts.map((item) => (
                    <option key={item}>{item}</option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Р”Р°С‚Р°</span>
                <input type="date" value={date} onChange={(event) => setDate(event.target.value)} />
              </label>
              <label className="field span-2">
                <span>РћРїРёСЃ РІРёС‚СЂР°С‚Рё</span>
                <textarea
                  rows="4"
                  placeholder="РљРѕРјРµРЅС‚Р°СЂ РїРѕ С†С–Р№ РІРёС‚СЂР°С‚С–"
                  value={description}
                  onChange={(event) => setDescription(event.target.value)}
                />
              </label>
            </div>

            <div className="expense-modal-actions">
              <button className="link-button" type="button" onClick={() => saveFinances('Р—Р°РїР»Р°РЅРѕРІР°РЅРѕ')}>
                Р—Р°РїР»Р°РЅСѓРІР°С‚Рё РІРёС‚СЂР°С‚Сѓ
              </button>
              <div>
                <button className="secondary-button" type="button" onClick={closeModal}>
                  Р—Р°РєСЂРёС‚Рё
                </button>
                <button className="primary-button" type="submit">
                  Р”РѕРґР°С‚Рё РІРёС‚СЂР°С‚Сѓ
                </button>
              </div>
            </div>
          </form>
        </div>
      )}
    </section>
  )
}

