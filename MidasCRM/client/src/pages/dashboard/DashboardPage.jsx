import { useMemo, useState } from 'react'
import { MetricCard } from '../../components/MetricCard.jsx'
import { OperationsTable } from '../../features/operations/components/OperationsTable.jsx'
import { OrdersTable } from '../../features/sales/components/OrdersTable.jsx'
import sharedStyles from '../../styles/Shared.module.css'
import pageStyles from '../../styles/pages/Dashboard.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



const periodLabels = {
  day: 'День',
  week: 'Тиждень',
  month: 'Місяць',
}

export function DashboardPage({ stats, sales, recentSales, operations }) {
  const [period, setPeriod] = useState('day')

  const periodStats = useMemo(() => {
    const multiplier = period === 'day' ? 1 : period === 'week' ? 3 : 8

    return {
      sales: stats.sales * multiplier,
      grossProfit: stats.grossProfit * multiplier,
      loss: stats.loss * multiplier,
    }
  }, [period, stats.grossProfit, stats.loss, stats.sales])

  const averageOrder = stats.sales > 0 ? Math.round(stats.revenue / stats.sales) : 0
  const conversionRate = stats.customers > 0 ? Math.round((stats.sales / stats.customers) * 100) : 0

  return (
    <section className={cx('page-stack')}>
      <div className={cx('page-header')}>
        <div className={cx('tabs')}>
          {Object.entries(periodLabels).map(([value, label]) => (
            <button
              key={value}
              type="button"
              className={period === value ? cx('tab-button', 'active') : cx('tab-button')}
              onClick={() => setPeriod(value)}
            >
              {label}
            </button>
          ))}
        </div>
      </div>

      <div className={cx('metric-grid')}>
        <MetricCard label="Статистика продажів" value={periodStats.sales} hint={`За період: ${periodLabels[period]}`} />
        <MetricCard
          label="Прибуток валовий"
          value={`${periodStats.grossProfit.toLocaleString('uk-UA')} грн`}
          hint="Продаж мінус собівартість і витрати"
        />
        <MetricCard
          label="Убиток/Витрати"
          value={`${periodStats.loss.toLocaleString('uk-UA')} грн`}
          hint="Зафіксовані витрати"
        />
        <MetricCard label="Товари" value={stats.products} hint="Активні позиції на складах" />
      </div>

      <div className={cx('insight-grid')}>
        <section className={cx('panel', 'insight-panel')}>
          <h2>Фінансовий огляд</h2>
          <div className={cx('insight-list')}>
            <span>
              <strong>{stats.revenue.toLocaleString('uk-UA')} грн</strong>
              Оборот
            </span>
            <span>
              <strong>{averageOrder.toLocaleString('uk-UA')} грн</strong>
              Середній чек
            </span>
            <span>
              <strong>{conversionRate}%</strong>
              Конверсія
            </span>
          </div>
        </section>

        <section className={cx('panel', 'insight-panel')}>
          <h2>Канали</h2>
          <div className={cx('channel-summary')}>
            <span>Усього продажів</span>
            <strong>{sales.length}</strong>
          </div>
          <div className={cx('channel-summary')}>
            <span>Товарів</span>
            <strong>{stats.products}</strong>
          </div>
        </section>
      </div>

      <section className={cx('panel')}>
        <h2>Останні продажі</h2>
        <OrdersTable orders={recentSales} />
      </section>

      <section className={cx('panel')}>
        <h2>Останні операції</h2>
        <OperationsTable operations={operations} />
      </section>
    </section>
  )
}

