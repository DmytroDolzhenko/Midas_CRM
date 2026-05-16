import { useState } from 'react'

export function useLocalStorage(key, initialValue) {
  const [value, setValue] = useState(() => {
    const storedValue = window.localStorage.getItem(key)
    return storedValue ? JSON.parse(storedValue) : initialValue
  })

  function updateValue(nextValue) {
    setValue((currentValue) => {
      const valueToStore = typeof nextValue === 'function' ? nextValue(currentValue) : nextValue

      if (valueToStore === null) {
        window.localStorage.removeItem(key)
        return valueToStore
      }

      window.localStorage.setItem(key, JSON.stringify(valueToStore))
      return valueToStore
    })
  }

  return [value, updateValue]
}
