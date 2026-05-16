import { loginRequest } from '../features/auth/api/authApi.js'
import { useLocalStorage } from './useLocalStorage.js'

export function useAuth() {
  const [user, setUser] = useLocalStorage('midas-user', null)

  async function login(credentials) {
    const nextUser = await loginRequest(credentials)
    setUser(nextUser)
  }
  function logout() {
    setUser(null)
  }

  return {
    user,
    login,
    logout,
  }
}
