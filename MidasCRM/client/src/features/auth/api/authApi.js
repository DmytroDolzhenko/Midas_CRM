export async function loginRequest({ email }) {
  return {
    email,
    name: email.split('@')[0] || 'manager',
    token: 'demo-token',
  }
}
