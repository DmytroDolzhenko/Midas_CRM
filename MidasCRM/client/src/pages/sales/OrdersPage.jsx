import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'

const statusTabs = ['Р’СЃС–', 'РџСЂРѕРґР°РЅРѕ', 'РџРѕРІРµСЂРЅРµРЅРЅСЏ']
const quickFilters = ['РЎСЊРѕРіРѕРґРЅС–', 'Р’С‡РѕСЂР°', 'РўРёР¶РґРµРЅСЊ', '30 РґРЅС–РІ', 'Р¦РµР№ РјС–СЃСЏС†СЊ', 'РњРёРЅСѓР»РёР№ РјС–СЃСЏС†СЊ', '3 РјС–СЃСЏС†С–']
const PAGE_SIZE = 10
const statusNames = {
  0: 'РћС‡С–РєСѓС”',
  1: 'Р’ РѕР±СЂРѕР±С†С–',
  2: 'Р’С–РґРїСЂР°РІР»РµРЅРѕ',
  3: 'Р”РѕСЃС‚Р°РІР»РµРЅРѕ',
  4: 'РџРѕРІРµСЂРЅРµРЅРЅСЏ',
  5: 'РћС‚СЂРёРјР°РЅРѕ',
  6: 'РЎРєР°СЃРѕРІР°РЅРѕ',
}

function ExportIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true">
      <path d="M12 3v12" />
      <path d="m7 10 5 5 5-5" />
      <path d="M5 21h14" />
    </svg>
  )
}

function formatStatus(status) {
  return statusNames[status] ?? status ?? 'РќРѕРІРёР№'
}

function formatDate(date) {
  return date.toISOString().slice(0, 10)
}

export function OrdersPage({ orders, onNavigate }) {
  const [activeStatus, setActiveStatus] = useState('Р’СЃС–')
  const [search, setSearch] = useState('')
  const [dateFrom, setDateFrom] = useState('2026-05-19')
  const [dateTo, setDateTo] = useState('2026-05-19')
  const [isFilterOpen, setIsFilterOpen] = useState(false)
  const [isMenuOpen, setIsMenuOpen] = useState(false)
  const [selectedOrder, setSelectedOrder] = useState(null)
  const [minTotal, setMinTotal] = useState('')
  const [maxTotal, setMaxTotal] = useState('')
  const [page, setPage] = useState(1)

  const filteredOrders = useMemo(
    () => {
      return orders.filter((order) => {
        const matchesSearch = `${order.code} ${order.customer} ${order.product} ${order.channel}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const isReturn = Number(order.status) === 4 || Number(order.status) === 6 || order.status === 'cancelled'
        const matchesStatus =
          activeStatus === 'Р’СЃС–' ||
          (activeStatus === 'РџСЂРѕРґР°РЅРѕ' && !isReturn) ||
          (activeStatus === 'РџРѕРІРµСЂРЅРµРЅРЅСЏ' && isReturn)
        const orderDate = order.date || ''
        const matchesDate = !orderDate || (orderDate >= dateFrom && orderDate <= dateTo)
        const matchesTotal =
          (!minTotal || Number(order.total) >= Number(minTotal)) &&
          (!maxTotal || Number(order.total) <= Number(maxTotal))

        return matchesSearch && matchesStatus && matchesDate && matchesTotal
      })
    },
    [activeStatus, dateFrom, dateTo, maxTotal, minTotal, orders, search],
  )
  const paginatedOrders = filteredOrders.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  const analytics = useMemo(() => {
    const total = filteredOrders.reduce((sum, order) => sum + Number(order.total), 0)
    const cost = filteredOrders.reduce((sum, order) => sum + Number(order.cost), 0)
    const profit = filteredOrders.reduce((sum, order) => sum + Number(order.profit), 0)
    const quantity = filteredOrders.reduce((sum, order) => sum + Number(order.quantity), 0)
    const markup = cost > 0 ? Math.round(((total - cost) / cost) * 100) : 0

    return { total, profit, quantity, markup }
  }, [filteredOrders])

  function exportSales() {
    const header = ['РџСЂРѕРґР°Р¶', 'РџРѕРєСѓРїРµС†СЊ', 'Р”РѕСЃС‚Р°РІРєР°', 'Р Р°С…СѓРЅРѕРє', 'РЎСѓРјР°', 'РЎС‚Р°С‚СѓСЃ']
    const rows = filteredOrders.map((order) => [
      order.code,
      order.customer,
      order.deliveryMode === 'nova-post' ? 'РќРѕРІР° РџРѕС€С‚Р°' : 'РџСЂРѕСЃС‚РёР№ РїСЂРѕРґР°Р¶',
      order.account || 'РќР°Р»РѕР¶РєР° NovaPay',
      order.total,
      formatStatus(order.status),
    ])
    const csv = [header, ...rows].map((row) => row.join(';')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')

    link.href = url
    link.download = 'midas-sales.csv'
    link.click()
    URL.revokeObjectURL(url)
  }

  function applyQuickFilter(filter) {
    const today = new Date('2026-05-19')
    const start = new Date(today)
    const end = new Date(today)

    if (filter === 'Р’С‡РѕСЂР°') {
      start.setDate(today.getDate() - 1)
      end.setDate(today.getDate() - 1)
    }

    if (filter === 'РўРёР¶РґРµРЅСЊ') {
      start.setDate(today.getDate() - 7)
    }

    if (filter === '30 РґРЅС–РІ') {
      start.setDate(today.getDate() - 30)
    }

    if (filter === 'Р¦РµР№ РјС–СЃСЏС†СЊ') {
      start.setDate(1)
    }

    if (filter === 'РњРёРЅСѓР»РёР№ РјС–СЃСЏС†СЊ') {
      start.setMonth(today.getMonth() - 1, 1)
      end.setMonth(today.getMonth(), 0)
    }

    if (filter === '3 РјС–СЃСЏС†С–') {
      start.setMonth(today.getMonth() - 3)
    }

    setDateFrom(formatDate(start))
    setDateTo(formatDate(end))
    setPage(1)
  }

  return (
    <section className="sales-page">
      <div className="sales-toolbar panel">
        <div className="sales-tabs-row">
          <div className="tabs">
            {statusTabs.map((tab) => (
              <button
                key={tab}
                type="button"
                className={activeStatus === tab ? 'tab-button active' : 'tab-button'}
                onClick={() => { setActiveStatus(tab); setPage(1) }}
              >
                {tab}
              </button>
            ))}
          </div>
          <button className="warehouse-add-button" type="button" onClick={() => onNavigate('createOrder')}>+</button>
          <div className="relative-menu">
            <button className="dots-button" type="button" onClick={() => setIsMenuOpen((isOpen) => !isOpen)}>...</button>
            {isMenuOpen && (
              <div className="floating-menu">
                <button type="button" onClick={exportSales}>Р•РєСЃРїРѕСЂС‚СѓРІР°С‚Рё РїСЂРѕРґР°Р¶С–</button>
                <button type="button" onClick={() => setIsFilterOpen(true)}>Р’С–РґРєСЂРёС‚Рё С„С–Р»СЊС‚СЂ</button>
                <button type="button" onClick={() => onNavigate('createOrder')}>РЎС‚РІРѕСЂРёС‚Рё РїСЂРѕРґР°Р¶</button>
              </div>
            )}
          </div>
        </div>

        <div className="sales-filter-grid">
          <input
            aria-label="РџРѕС€СѓРєРѕРІР° С„СЂР°Р·Р°"
            placeholder="РџРѕС€СѓРєРѕРІР° С„СЂР°Р·Р°"
            value={search}
            onChange={(event) => { setSearch(event.target.value); setPage(1) }}
          />
          <input type="date" value={dateFrom} onChange={(event) => { setDateFrom(event.target.value); setPage(1) }} />
          <input type="date" value={dateTo} onChange={(event) => { setDateTo(event.target.value); setPage(1) }} />
          <button className="icon-button flat-icon-button" type="button" aria-label="Р•РєСЃРїРѕСЂС‚" onClick={exportSales}>
            <ExportIcon />
          </button>
          <button className="secondary-button" type="button" onClick={() => setIsFilterOpen((isOpen) => !isOpen)}>Р¤С–Р»СЊС‚СЂ</button>
        </div>

        <div className="quick-filter-row">
          {quickFilters.map((filter) => (
            <button key={filter} type="button" onClick={() => applyQuickFilter(filter)}>{filter}</button>
          ))}
        </div>

        {isFilterOpen && (
          <div className="inline-filter-panel">
            <label className="field">
              <span>РЎС‚Р°С‚СѓСЃ</span>
              <select value={activeStatus} onChange={(event) => { setActiveStatus(event.target.value); setPage(1) }}>
                {statusTabs.map((tab) => (
                  <option key={tab}>{tab}</option>
                ))}
              </select>
            </label>
            <label className="field">
              <span>РџРѕС€СѓРє</span>
              <input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1) }} />
            </label>
            <label className="field">
              <span>РЎСѓРјР° РІС–Рґ</span>
              <input min="0" type="number" value={minTotal} onChange={(event) => { setMinTotal(event.target.value); setPage(1) }} />
            </label>
            <label className="field">
              <span>РЎСѓРјР° РґРѕ</span>
              <input min="0" type="number" value={maxTotal} onChange={(event) => { setMaxTotal(event.target.value); setPage(1) }} />
            </label>
          </div>
        )}
      </div>

      <div className="inventory-widget-grid">
        <article><span>РџСЂРѕРґР°Р¶С–РІ Р·Р°РіР°Р»РѕРј</span><strong>{filteredOrders.length} ({analytics.quantity.toFixed(2)} РѕРґ.)</strong></article>
        <article><span>РџСЂРѕРґР°РЅРѕ РЅР°</span><strong>{analytics.total.toLocaleString('uk-UA')} РіСЂРЅ.</strong></article>
        <article><span>Р—Р°СЂРѕР±Р»РµРЅРѕ</span><strong>{analytics.profit.toLocaleString('uk-UA')} РіСЂРЅ.</strong></article>
        <article><span>РќР°С†С–РЅРєР°</span><strong>{analytics.markup || '-'}%</strong></article>
        <article><span>РљРѕРјС–СЃС–СЏ</span><strong>0.00 РіСЂРЅ.</strong></article>
      </div>

      <section className="sales-table-card panel">
        <div className="sales-layout-header">
          <span>РџСЂРѕРґР°Р¶</span>
          <span>РџРѕРєСѓРїРµС†СЊ</span>
          <span>Р”РѕСЃС‚Р°РІРєР°</span>
          <span>Р Р°С…СѓРЅРѕРє</span>
          <span>РЎСѓРјР°, РіСЂРЅ.</span>
          <span>РЎС‚Р°С‚СѓСЃ</span>
          <span>Р”С–С—</span>
        </div>

        {filteredOrders.length === 0 ? (
          <div className="sales-empty-state">
            <h2>РќРµРјР°С” РїСЂРѕРґР°Р¶С–РІ</h2>
            <button className="primary-button" type="button" onClick={() => onNavigate('createOrder')}>
              Р”РѕРґР°С‚Рё РїРµСЂС€РёР№ РїСЂРѕРґР°Р¶
            </button>
          </div>
        ) : (
          paginatedOrders.map((order) => (
            <div className="sales-layout-row" key={order.id}>
              <strong>{order.code}</strong>
              <span>{order.customer}</span>
              <span>{order.deliveryMode === 'nova-post' ? 'РќРѕРІР° РџРѕС€С‚Р°' : 'РџСЂРѕСЃС‚РёР№ РїСЂРѕРґР°Р¶'}</span>
              <span>{order.account || 'РќР°Р»РѕР¶РєР° NovaPay'}</span>
              <span>{Number(order.total).toLocaleString('uk-UA')}</span>
              <span>{formatStatus(order.status)}</span>
              <button type="button" onClick={() => setSelectedOrder(order)}>Р’С–РґРєСЂРёС‚Рё</button>
            </div>
          ))
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredOrders.length} onPageChange={setPage} />
      </section>

      {selectedOrder && (
        <div className="modal-backdrop" role="presentation" onClick={() => setSelectedOrder(null)}>
          <section className="small-modal" role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className="settings-header">
              <div>
                <p className="eyebrow">Sale</p>
                <h2>{selectedOrder.code}</h2>
              </div>
              <button className="modal-close-button" type="button" onClick={() => setSelectedOrder(null)}>Г—</button>
            </div>
            <div className="settings-grid">
              <label className="field">
                <span>РџРѕРєСѓРїРµС†СЊ</span>
                <input readOnly value={selectedOrder.customer} />
              </label>
              <label className="field">
                <span>РўРѕРІР°СЂ</span>
                <input readOnly value={selectedOrder.product} />
              </label>
              <label className="field">
                <span>РљР°РЅР°Р»</span>
                <input readOnly value={selectedOrder.channel} />
              </label>
              <label className="field">
                <span>РЎСѓРјР°</span>
                <input readOnly value={`${Number(selectedOrder.total).toLocaleString('uk-UA')} РіСЂРЅ.`} />
              </label>
            </div>
            <div className="settings-actions">
              <button className="primary-button" type="button" onClick={() => setSelectedOrder(null)}>Р—Р°РєСЂРёС‚Рё</button>
            </div>
          </section>
        </div>
      )}
    </section>
  )
}


