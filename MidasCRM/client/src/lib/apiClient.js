const API_URL = import.meta.env.VITE_API_URL ?? 'https://localhost:7240/api'

function getToken() {
  try {
    const savedUser = localStorage.getItem('midas-user')
    const user = savedUser ? JSON.parse(savedUser) : null
    return user?.token ?? null
  } catch {
    return null
  }
}

export async function apiRequest(path, options = {}) {
  const token = getToken()
  const isFormData = options.body instanceof FormData
  const body = isFormData || typeof options.body === 'string'
    ? options.body
    : options.body
      ? JSON.stringify(options.body)
      : undefined
  const headers = {
    Accept: 'application/json',
    ...(isFormData ? {} : { 'Content-Type': 'application/json' }),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  }

  let response

  try {
    response = await fetch(`${API_URL}${path}`, {
      ...options,
      body,
      headers,
    })
  } catch {
    throw new Error(`Не вдалося з'єднатися з сервером: ${API_URL}`)
  }

  if (!response.ok) {
    let message = `API request failed: ${response.status}`

    try {
      const error = await response.clone().json()
      message = error.message ?? error.Message ?? JSON.stringify(error)
    } catch {
      message = await response.text() || message
    }

    throw new Error(message)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}
