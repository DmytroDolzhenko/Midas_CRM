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

function getActiveCompanyId() {
  try {
    return localStorage.getItem('midas-active-company-id')
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
    ...(getActiveCompanyId() ? { 'X-Company-Id': getActiveCompanyId() } : {}),
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
    throw new Error(`Unable to connect to server: ${API_URL}`)
  }

  if (!response.ok) {
    const handleUnauthorized = options.handleUnauthorized !== false

    if (response.status === 401 && handleUnauthorized) {
      const error = new Error('Session expired. Please log in again.')
      error.status = response.status
      localStorage.removeItem('midas-user')
      window.dispatchEvent(new Event('midas-auth-expired'))
      throw error
    }

    let message = `API request failed: ${response.status}`

    try {
      const errorPayload = await response.clone().json()
      message = errorPayload.message ?? errorPayload.Message ?? JSON.stringify(errorPayload)
    } catch {
      message = await response.text() || message
    }

    const error = new Error(message)
    error.status = response.status
    throw error
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}
