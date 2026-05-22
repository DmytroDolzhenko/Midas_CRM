import { useEffect } from 'react'
import { loginRequest } from '../features/auth/api/authApi.js'
import { useLocalStorage } from './useLocalStorage.js'

export function useAuth() {
  const [user, setUser] = useLocalStorage('midas-user', null)

  async function login(credentials) {
    const nextUser = await loginRequest(credentials)
    setUser(nextUser)
  }
  async function register(credentials) {
    const nextUser = await registerRequest(credentials)
    setUser(nextUser)
  }

  function logout() {
    setUser(null)
  }

  useEffect(() => {
    function handleAuthExpired() {
      setUser(null)
    }

    window.addEventListener('midas-auth-expired', handleAuthExpired)
    return () => window.removeEventListener('midas-auth-expired', handleAuthExpired)
  }, [setUser])

  return {
    user,
    login,
    register,
    logout,
  }
}
