import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Sales.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



const statusTabs = ['Всі', 'Продано', 'Повернення']
const quickFilters = ['Сьогодні', 'Вчора', 'Тиждень', '30 днів', 'Цей місяць', 'Минулий місяць', '3 місяці']
const PAGE_SIZE = 10
const statusNames = {
  0: 'Очікує',
  1: 'В обробці',
  2: 'Відправлено',
  3: 'Доставлено',
  4: 'Повернення',
  5: 'Отримано',
  6: 'Скасовано',
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
  return statusNames[status] ?? status ?? 'Новий'
}

function formatDate(date) {
  return date.toISOString().slice(0, 10)
}

export function OrdersPage({ orders, onNavigate }) {
  const [activeStatus, setActiveStatus] = useState('Всі')
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
          activeStatus === 'Всі' ||
          (activeStatus === 'Продано' && !isReturn) ||
          (activeStatus === 'Повернення' && isReturn)
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
    const header = ['Продаж', 'Покупець', 'Доставка', '� ахунок', 'Сума', 'Статус']
    const rows = filteredOrders.map((order) => [
      order.code,
      order.customer,
      order.deliveryMode === 'nova-post' ? 'Нова Пошта' : 'Простий продаж',
      order.account || 'Наложка NovaPay',
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

    if (filter === 'Вчора') {
      start.setDate(today.getDate() - 1)
      end.setDate(today.getDate() - 1)
    }

    if (filter === 'Тиждень') {
      start.setDate(today.getDate() - 7)
    }

    if (filter === '30 днів') {
      start.setDate(today.getDate() - 30)
    }

    if (filter === 'Цей місяць') {
      start.setDate(1)
    }

    if (filter === 'Минулий місяць') {
      start.setMonth(today.getMonth() - 1, 1)
      end.setMonth(today.getMonth(), 0)
    }

    if (filter === '3 місяці') {
      start.setMonth(today.getMonth() - 3)
    }

    setDateFrom(formatDate(start))
    setDateTo(formatDate(end))
    setPage(1)
  }

  return (
    <section className={cx('sales-page')}>
      <div className={cx('sales-toolbar', 'panel')}>
        <div className={cx('sales-tabs-row')}>
          <div className={cx('tabs')}>
            {statusTabs.map((tab) => (
              <button
                key={tab}
                type="button"
                className={activeStatus === tab ? cx('tab-button', 'active') : cx('tab-button')}
                onClick={() => { setActiveStatus(tab); setPage(1) }}
              >
                {tab}
              </button>
            ))}
          </div>
          <button className={cx('warehouse-add-button')} type="button" onClick={() => onNavigate('createOrder')}>+</button>
          <div className={cx('relative-menu')}>
            <button className={cx('dots-button')} type="button" onClick={() => setIsMenuOpen((isOpen) => !isOpen)}>...</button>
            {isMenuOpen && (
              <div className={cx('floating-menu')}>
                <button type="button" onClick={exportSales}>Експортувати продажі</button>
                <button type="button" onClick={() => setIsFilterOpen(true)}>Відкрити фільтр</button>
                <button type="button" onClick={() => onNavigate('createOrder')}>Створити продаж</button>
              </div>
            )}
          </div>
        </div>

        <div className={cx('sales-filter-grid')}>
          <input
            aria-label="Пошукова фраза"
            placeholder="Пошукова фраза"
            value={search}
            onChange={(event) => { setSearch(event.target.value); setPage(1) }}
          />
          <input type="date" value={dateFrom} onChange={(event) => { setDateFrom(event.target.value); setPage(1) }} />
          <input type="date" value={dateTo} onChange={(event) => { setDateTo(event.target.value); setPage(1) }} />
          <button className={cx('icon-button', 'flat-icon-button')} type="button" aria-label="Експорт" onClick={exportSales}>
            <ExportIcon />
          </button>
          <button className={cx('secondary-button')} type="button" onClick={() => setIsFilterOpen((isOpen) => !isOpen)}>Фільтр</button>
        </div>

        <div className={cx('quick-filter-row')}>
          {quickFilters.map((filter) => (
            <button key={filter} type="button" onClick={() => applyQuickFilter(filter)}>{filter}</button>
          ))}
        </div>

        {isFilterOpen && (
          <div className={cx('inline-filter-panel')}>
            <label className={cx('field')}>
              <span>Статус</span>
              <select value={activeStatus} onChange={(event) => { setActiveStatus(event.target.value); setPage(1) }}>
                {statusTabs.map((tab) => (
                  <option key={tab}>{tab}</option>
                ))}
              </select>
            </label>
            <label className={cx('field')}>
              <span>Пошук</span>
              <input value={search} onChange={(event) => { setSearch(event.target.value); setPage(1) }} />
            </label>
            <label className={cx('field')}>
              <span>Сума від</span>
              <input min="0" type="number" value={minTotal} onChange={(event) => { setMinTotal(event.target.value); setPage(1) }} />
            </label>
            <label className={cx('field')}>
              <span>Сума до</span>
              <input min="0" type="number" value={maxTotal} onChange={(event) => { setMaxTotal(event.target.value); setPage(1) }} />
            </label>
          </div>
        )}
      </div>

      <div className={cx('inventory-widget-grid')}>
        <article><span>Продажів загалом</span><strong>{filteredOrders.length} ({analytics.quantity.toFixed(2)} од.)</strong></article>
        <article><span>Продано на</span><strong>{analytics.total.toLocaleString('uk-UA')} грн.</strong></article>
        <article><span>Зароблено</span><strong>{analytics.profit.toLocaleString('uk-UA')} грн.</strong></article>
        <article><span>Націнка</span><strong>{analytics.markup || '-'}%</strong></article>
        <article><span>Комісія</span><strong>0.00 грн.</strong></article>
      </div>

      <section className={cx('sales-table-card', 'panel')}>
        <div className={cx('sales-layout-header')}>
          <span>Продаж</span>
          <span>Покупець</span>
          <span>Доставка</span>
          <span>� ахунок</span>
          <span>Сума, грн.</span>
          <span>Статус</span>
          <span>Дії</span>
        </div>

        {filteredOrders.length === 0 ? (
          <div className={cx('sales-empty-state')}>
            <h2>Немає продажів</h2>
            <button className={cx('primary-button')} type="button" onClick={() => onNavigate('createOrder')}>
              Додати перший продаж
            </button>
          </div>
        ) : (
          paginatedOrders.map((order) => (
            <div className={cx('sales-layout-row')} key={order.id}>
              <strong>{order.code}</strong>
              <span>{order.customer}</span>
              <span>{order.deliveryMode === 'nova-post' ? 'Нова Пошта' : 'Простий продаж'}</span>
              <span>{order.account || 'Наложка NovaPay'}</span>
              <span>{Number(order.total).toLocaleString('uk-UA')}</span>
              <span>{formatStatus(order.status)}</span>
              <button type="button" onClick={() => setSelectedOrder(order)}>Відкрити</button>
            </div>
          ))
        )}
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredOrders.length} onPageChange={setPage} />
      </section>

      {selectedOrder && (
        <div className={cx('modal-backdrop')} role="presentation" onClick={() => setSelectedOrder(null)}>
          <section className={cx('small-modal')} role="dialog" aria-modal="true" onClick={(event) => event.stopPropagation()}>
            <div className={cx('settings-header')}>
              <div>
                <p className={cx('eyebrow')}>Sale</p>
                <h2>{selectedOrder.code}</h2>
              </div>
              <button className={cx('modal-close-button')} type="button" onClick={() => setSelectedOrder(null)}>×</button>
            </div>
            <div className={cx('settings-grid')}>
              <label className={cx('field')}>
                <span>Покупець</span>
                <input readOnly value={selectedOrder.customer} />
              </label>
              <label className={cx('field')}>
                <span>Товар</span>
                <input readOnly value={selectedOrder.product} />
              </label>
              <label className={cx('field')}>
                <span>Канал</span>
                <input readOnly value={selectedOrder.channel} />
              </label>
              <label className={cx('field')}>
                <span>Сума</span>
                <input readOnly value={`${Number(selectedOrder.total).toLocaleString('uk-UA')} грн.`} />
              </label>
            </div>
            <div className={cx('settings-actions')}>
              <button className={cx('primary-button')} type="button" onClick={() => setSelectedOrder(null)}>Закрити</button>
            </div>
          </section>
        </div>
      )}
    </section>
  )
}


