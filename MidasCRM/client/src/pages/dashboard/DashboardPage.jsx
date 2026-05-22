import { useMemo, useState } from 'react'
import { MetricCard } from '../../components/MetricCard.jsx'
import { OperationsTable } from '../../features/operations/components/OperationsTable.jsx'
import { OrdersTable } from '../../features/sales/components/OrdersTable.jsx'

const periodLabels = {
  day: 'Р”РµРЅСЊ',
  week: 'РўРёР¶РґРµРЅСЊ',
  month: 'РњС–СЃСЏС†СЊ',
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
    <section className="page-stack">
      <div className="page-header">
        <div className="tabs">
          {Object.entries(periodLabels).map(([value, label]) => (
            <button
              key={value}
              type="button"
              className={period === value ? 'tab-button active' : 'tab-button'}
              onClick={() => setPeriod(value)}
            >
              {label}
            </button>
          ))}
        </div>
      </div>

      <div className="metric-grid">
        <MetricCard label="РЎС‚Р°С‚РёСЃС‚РёРєР° РїСЂРѕРґР°Р¶С–РІ" value={periodStats.sales} hint={`Р—Р° РїРµСЂС–РѕРґ: ${periodLabels[period]}`} />
        <MetricCard
          label="РџСЂРёР±СѓС‚РѕРє РІР°Р»РѕРІРёР№"
          value={`${periodStats.grossProfit.toLocaleString('uk-UA')} РіСЂРЅ`}
          hint="РџСЂРѕРґР°Р¶ РјС–РЅСѓСЃ СЃРѕР±С–РІР°СЂС‚С–СЃС‚СЊ С– РІРёС‚СЂР°С‚Рё"
        />
        <MetricCard
          label="РЈР±РёС‚РѕРє/Р’РёС‚СЂР°С‚Рё"
          value={`${periodStats.loss.toLocaleString('uk-UA')} РіСЂРЅ`}
          hint="Р—Р°С„С–РєСЃРѕРІР°РЅС– РІРёС‚СЂР°С‚Рё"
        />
        <MetricCard label="РўРѕРІР°СЂРё" value={stats.products} hint="РђРєС‚РёРІРЅС– РїРѕР·РёС†С–С— РЅР° СЃРєР»Р°РґР°С…" />
      </div>

      <div className="insight-grid">
        <section className="panel insight-panel">
          <h2>Р¤С–РЅР°РЅСЃРѕРІРёР№ РѕРіР»СЏРґ</h2>
          <div className="insight-list">
            <span>
              <strong>{stats.revenue.toLocaleString('uk-UA')} РіСЂРЅ</strong>
              РћР±РѕСЂРѕС‚
            </span>
            <span>
              <strong>{averageOrder.toLocaleString('uk-UA')} РіСЂРЅ</strong>
              РЎРµСЂРµРґРЅС–Р№ С‡РµРє
            </span>
            <span>
              <strong>{conversionRate}%</strong>
              РљРѕРЅРІРµСЂСЃС–СЏ
            </span>
          </div>
        </section>

        <section className="panel insight-panel">
          <h2>РљР°РЅР°Р»Рё</h2>
          <div className="channel-summary">
            <span>РЈСЃСЊРѕРіРѕ РїСЂРѕРґР°Р¶С–РІ</span>
            <strong>{sales.length}</strong>
          </div>
          <div className="channel-summary">
            <span>РўРѕРІР°СЂС–РІ</span>
            <strong>{stats.products}</strong>
          </div>
        </section>
      </div>

      <section className="panel">
        <h2>РћСЃС‚Р°РЅРЅС– РїСЂРѕРґР°Р¶С–</h2>
        <OrdersTable orders={recentSales} />
      </section>

      <section className="panel">
        <h2>РћСЃС‚Р°РЅРЅС– РѕРїРµСЂР°С†С–С—</h2>
        <OperationsTable operations={operations} />
      </section>
    </section>
  )
}

