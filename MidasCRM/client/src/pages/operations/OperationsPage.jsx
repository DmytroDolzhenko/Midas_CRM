import { useMemo, useState } from 'react'
import { Pagination } from '../../components/Pagination.jsx'
import { OperationsTable } from '../../features/operations/components/OperationsTable.jsx'

const PAGE_SIZE = 10

export function OperationsPage({ operations }) {
  const [search, setSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState('all')
  const [dateFrom, setDateFrom] = useState('')
  const [dateTo, setDateTo] = useState('')
  const [page, setPage] = useState(1)
  const operationTypes = useMemo(
    () => Array.from(new Set(operations.map((operation) => operation.type).filter(Boolean))),
    [operations],
  )
  const filteredOperations = useMemo(
    () =>
      operations.filter((operation) => {
        const matchesSearch = `${operation.type ?? ''} ${operation.description ?? ''} ${operation.actor ?? ''} ${operation.amount ?? ''}`
          .toLowerCase()
          .includes(search.toLowerCase())
        const matchesType = typeFilter === 'all' || operation.type === typeFilter
        const operationDate = String(operation.createdAt ?? '').slice(0, 10)
        const matchesDate = (!dateFrom || operationDate >= dateFrom) && (!dateTo || operationDate <= dateTo)

        return matchesSearch && matchesType && matchesDate
      }),
    [dateFrom, dateTo, operations, search, typeFilter],
  )
  const paginatedOperations = filteredOperations.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE)

  function updateFilter(setter, value) {
    setter(value)
    setPage(1)
  }

  return (
    <section className="page-stack">
      <section className="panel">
        <div className="table-filter-grid">
          <input
            aria-label="РџРѕС€СѓРє С–СЃС‚РѕСЂС–С—"
            placeholder="РџРѕС€СѓРє РІ С–СЃС‚РѕСЂС–С—"
            value={search}
            onChange={(event) => updateFilter(setSearch, event.target.value)}
          />
          <select value={typeFilter} onChange={(event) => updateFilter(setTypeFilter, event.target.value)}>
            <option value="all">РЈСЃС– С‚РёРїРё</option>
            {operationTypes.map((type) => (
              <option key={type} value={type}>{type}</option>
            ))}
          </select>
          <input type="date" value={dateFrom} onChange={(event) => updateFilter(setDateFrom, event.target.value)} />
          <input type="date" value={dateTo} onChange={(event) => updateFilter(setDateTo, event.target.value)} />
        </div>
        <OperationsTable operations={paginatedOperations} />
        <Pagination page={page} pageSize={PAGE_SIZE} total={filteredOperations.length} onPageChange={setPage} />
      </section>
    </section>
  )
}

