import { serverApi } from '../../../lib/serverApi.js'

export async function loginRequest(credentials) {
  const response = await serverApi.auth.login(credentials)
  const email = response.email ?? response.Email ?? credentials.email
  const token = response.token ?? response.Token

  return {
    email,
    name: email.split('@')[0] || 'manager',
    token,
  }
}
