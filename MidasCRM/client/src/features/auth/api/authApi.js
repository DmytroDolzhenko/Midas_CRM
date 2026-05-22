import { serverApi } from '../../../lib/serverApi.js'

function decodeJwtPayload(token) {
  try {
    const payload = token.split('.')[1]
    return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
  } catch {
    return null
  }
}

export async function loginRequest(credentials) {
  const response = await serverApi.auth.login(credentials)
  const email = response.email ?? response.Email ?? credentials.email
  const token = response.token ?? response.Token
  const payload = decodeJwtPayload(token)

  return {
    email,
    name: email.split('@')[0] || 'manager',
    token,
    id: payload?.sub ?? payload?.nameid ?? payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
  }
}
