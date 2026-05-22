import { apiRequest } from './apiClient.js'

export const serverApi = {
  auth: {
    login(credentials) {
      return apiRequest('/Auth/login', {
        method: 'POST',
        body: {
          email: credentials.email,
          password: credentials.password,
        },
      })
    },
    register(credentials) {
      return apiRequest('/Auth/register', {
        method: 'POST',
        body: {
          email: credentials.email,
          phoneNumber: credentials.phoneNumber,
          password: credentials.password,
          name: credentials.name,
          surname: credentials.surname,
          fathername: credentials.fathername,
        },
      })
    },
  },
  companies: {
    getAll() {
      return apiRequest('/Company')
    },
    getById(id) {
      return apiRequest(`/Company/${id}`)
    },
    getBalance(options = {}) {
      return apiRequest('/Company/balance', options)
    },
    create(company) {
      return apiRequest('/Company', {
        method: 'POST',
        body: company,
      })
    },
    update(id, company) {
      return apiRequest(`/Company/${id}`, {
        method: 'PUT',
        body: company,
      })
    },
    remove(id) {
      return apiRequest(`/Company/${id}`, {
        method: 'DELETE',
      })
    },
    addMemberByEmail(companyId, email) {
      return apiRequest('/Company/add-member', {
        method: 'POST',
        body: { companyId, email },
      })
    },
  },
  companyMembers: {
    getAll() {
      return apiRequest('/CompanyMember')
    },
    updateRole(userId, role) {
      return apiRequest(`/CompanyMember/${userId}/role`, {
        method: 'PUT',
        body: { role },
      })
    },
    remove(userId) {
      return apiRequest(`/CompanyMember/${userId}`, {
        method: 'DELETE',
      })
    },
  },
  products: {
    getAll() {
      return apiRequest('/Product')
    },
    create(product) {
      return apiRequest('/Product', {
        method: 'POST',
        body: product,
      })
    },
    createWithVariants(product) {
      return apiRequest('/Product/product-with-variants', {
        method: 'POST',
        body: product,
      })
    },
  },
  productVariants: {
    getAll() {
      return apiRequest('/ProductVariant')
    },
    create(variant) {
      return apiRequest('/ProductVariant', {
        method: 'POST',
        body: variant,
      })
    },
  },
  orders: {
    getAll() {
      return apiRequest('/Order')
    },
    createOneClick(order) {
      return apiRequest('/Order/one-click', {
        method: 'POST',
        body: order,
      })
    },
  },
  customers: {
    getAll() {
      return apiRequest('/Customer')
    },
  },
  categories: {
    getAll() {
      return apiRequest('/ProductCategory')
    },
  },
  warehouses: {
    getAll() {
      return apiRequest('/Warehouse')
    },
    create(warehouse) {
      return apiRequest('/Warehouse', {
        method: 'POST',
        body: warehouse,
      })
    },
    update(id, warehouse) {
      return apiRequest(`/Warehouse/${id}`, {
        method: 'PUT',
        body: warehouse,
      })
    },
  },
}
