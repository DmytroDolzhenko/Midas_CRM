import { useMemo, useState } from 'react'

export function useSalesSearch(items, getSearchText) {
  const [search, setSearch] = useState('')

  const filteredItems = useMemo(
    () =>
      items.filter((item) =>
        getSearchText(item).toLowerCase().includes(search.toLowerCase()),
      ),
    [getSearchText, items, search],
  )

  return {
    search,
    setSearch,
    filteredItems,
  }
}
